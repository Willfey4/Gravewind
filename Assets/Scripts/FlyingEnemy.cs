using System.Collections;
using UnityEngine;

/// <summary>
/// FlyingEnemy — Hover and swoop attack behavior only.
/// Patrol, detection, sprite flipping, and death are all handled by EnemyManager.
///
/// REQUIRES on this GameObject:
///   - EnemyManager    (shared enemy brain)
///   - Health          (tagged "Enemy")
///   - Rigidbody2D     (Gravity Scale = 0 — script enforces this)
///   - Animator
///
/// SETUP NOTES:
///   - Place EnemyManager patrol points in the air at your desired flight height.
///   - Set retreatHeight to match that same Y position.
///
/// ANIMATOR PARAMETERS:
///   "IsFlying"   (bool) — always true while alive
///   "IsSwooping" (bool) — true during dive
///   "IsAlive"    (bool) — set false on death (EnemyManager handles this)
/// </summary>
public class FlyingEnemy : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] float bobAmplitude = 0.3f;
    [SerializeField] float bobFrequency = 1.5f;

    [Header("Swoop Attack")]
    [Tooltip("Empty child at the enemy's body centre — used for hit detection during swoop.")]
    [SerializeField] Transform attackHitbox;
    [SerializeField] Vector2 attackHitboxSize = new Vector2(0.8f, 0.8f);
    [SerializeField] int swoopDamage = 3;
    [SerializeField] float swoopDiveSpeed = 6f;
    [SerializeField] float swoopRetreatSpeed = 4f;
    [Tooltip("World-space Y to return to after swooping. Match your patrol points' height.")]
    [SerializeField] float retreatHeight = 5f;
    [SerializeField] float swoopCooldown = 2.5f;
    [SerializeField] LayerMask groundLayer;

    [Header("Knockback")]
    [SerializeField] Vector2 knockbackForce = new Vector2(2f, 5f);

    
    
    private enum State { Patrolling, Hovering, Swooping, Retreating }
    private State currentState = State.Patrolling;


    private EnemyManager em;
    private Animator anim;
    private Rigidbody2D rb;
    private float swoopTimer = 0f;
    private float bobTimer = 0f;

    private void Awake()
    {
        em   = GetComponent<EnemyManager>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Flying enemy ignores gravity while alive
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (em.isDead) return;

        swoopTimer -= Time.deltaTime;
        bobTimer   += Time.deltaTime;

        switch (currentState)
        {
            case State.Patrolling: UpdatePatrol(); break;
            case State.Hovering:  UpdateHover();  break;
            // Swooping and Retreating are driven by coroutines
        }
    }

    // ── States ────────────────────────────────────────────────────────────

    private void UpdatePatrol()
    {
        anim.SetBool("IsFlying", true);

        // Add a vertical bob on top of EnemyManager's horizontal patrol
        em.Patrol();
        float bobY = Mathf.Sin(bobTimer * bobFrequency) * bobAmplitude;
        em.SetVelocity(new Vector2(em.GetRigidbody().linearVelocity.x, bobY));

        if (em.CheckDetectionCircle())
        {
            em.StopHorizontal();
            currentState = State.Hovering;
        }
    }

    private void UpdateHover()
    {
        em.CheckDetectionCircle();      // drop aggro if player leaves range

        if (em.player == null)
        {
            currentState = State.Patrolling;
            return;
        }

        // Gentle bob while waiting to swoop
        float bobY = Mathf.Sin(bobTimer * bobFrequency) * (bobAmplitude * 0.5f);
        em.SetVelocity(new Vector2(0f, bobY));

        em.FacePlayer();

        if (swoopTimer <= 0f)
            StartCoroutine(PerformSwoop());
    }

    // ── Swoop ─────────────────────────────────────────────────────────────

    private IEnumerator PerformSwoop()
    {
        swoopTimer = swoopCooldown;
        currentState = State.Swooping;
        anim.SetBool("IsSwooping", true);

        bool hitPlayer = false;

        while (currentState == State.Swooping)
        {
            if (em.player == null) break;

            Vector2 dir = ((Vector2)em.player.position - (Vector2)transform.position).normalized;
            em.SetVelocity(dir * swoopDiveSpeed);

            // Hit check — same OverlapBoxAll pattern as PlayerManager.hit()
            if (attackHitbox != null && !hitPlayer)
            {
                Collider2D[] hits = Physics2D.OverlapBoxAll(attackHitbox.position, attackHitboxSize, 0f);
                foreach (Collider2D hit in hits)
                {
                    if (hit.CompareTag("Player"))
                    {
                        hitPlayer = true;
                        hit.GetComponent<Health>().TakeDamage(swoopDamage);

                        Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
                        if (playerRb != null)
                        {
                            float kbDir = hit.transform.position.x > transform.position.x ? 1f : -1f;
                            playerRb.linearVelocity = Vector2.zero;
                            playerRb.AddForce(new Vector2(knockbackForce.x * kbDir, knockbackForce.y),
                                              ForceMode2D.Impulse);
                        }
                        break;
                    }
                }
                if (hitPlayer) break;
            }

            // Stop if we hit terrain
            RaycastHit2D ground = Physics2D.Raycast(transform.position, dir, 0.5f, groundLayer);
            if (ground.collider != null) break;

            if (Vector2.Distance(transform.position, em.player.position) < 0.5f) break;

            yield return null;
        }

        anim.SetBool("IsSwooping", false);
        yield return StartCoroutine(Retreat());
    }

    private IEnumerator Retreat()
    {
        currentState = State.Retreating;

        while (transform.position.y < retreatHeight - 0.1f)
        {
            em.SetVelocity(new Vector2(0f, swoopRetreatSpeed));
            yield return null;
        }

        em.SetVelocity(Vector2.zero);
        currentState = em.player != null ? State.Hovering : State.Patrolling;
    }

    // ── Gizmos ────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        // Retreat height line
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(transform.position.x - 3f, retreatHeight),
                        new Vector3(transform.position.x + 3f, retreatHeight));

        if (attackHitbox != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawWireCube(attackHitbox.position, attackHitboxSize);
        }
    }
}
