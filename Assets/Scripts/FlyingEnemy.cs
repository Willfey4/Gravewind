using System.Collections;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Float")]
    [SerializeField] float bobAmplitude = 0.3f;
    [SerializeField] float bobFrequency = 1.5f;
 
    [Header("Swoop Attack")]
    [SerializeField] float swoopDiveSpeed = 6f;
    [SerializeField] float swoopRetreatSpeed = 4f;
    [Tooltip("World-space Y to return to after swooping. Match your patrol points' height.")]
    [SerializeField] float retreatHeight = 5f;
    [SerializeField] float swoopCooldown = 2.5f;
    [SerializeField] LayerMask groundLayer;
 
 
    private enum State { Patrolling, Hovering, Swooping, Retreating }
    private State currentState = State.Patrolling;
 
    private EnemyManager em;
    private Animator anim;
    private Rigidbody2D rb;
    private float swoopTimer = 0f;
    private float bobTimer = 0f;
    private bool hitPlayerThisSwoop = false;
 
    private void Awake()
    {
        em  = GetComponent<EnemyManager>();
        anim = GetComponent<Animator>();
        rb  = GetComponent<Rigidbody2D>();
 
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
            case State.Hovering:   UpdateHover();  break;
            // Swooping and Retreating are driven by coroutines
        }
    }
 
    // ── Collision — triggers retreat if we hit the player mid-swoop ───────
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (currentState == State.Swooping && other.gameObject.CompareTag("Player"))
        {
            hitPlayerThisSwoop = true;
        }
    }
 
    // ── States ────────────────────────────────────────────────────────────
 
    private void UpdatePatrol()
    {
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
        em.CheckDetectionCircle(); // drop aggro if player leaves range
 
        if (em.player == null)
        {
            currentState = State.Patrolling;
            return;
        }
 
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
        hitPlayerThisSwoop = false;
        anim.SetBool("IsSwooping", true);
 
        while (currentState == State.Swooping)
        {
            if (em.isDead) yield break;
 
            if (em.player == null) break;
 
            // Retreat as soon as we touch the player — EnemyManager handles the damage
            if (hitPlayerThisSwoop) break;
 
            Vector2 dir = ((Vector2)em.player.position - (Vector2)transform.position).normalized;
            em.SetVelocity(dir * swoopDiveSpeed);
 
            // Stop if we hit terrain
            RaycastHit2D ground = Physics2D.Raycast(transform.position, dir, 0.5f, groundLayer);
            if (ground.collider != null) break;
 
            // Close enough to player — retreat
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
            if (em.isDead) yield break;
            em.SetVelocity(new Vector2(0f, swoopRetreatSpeed));
            yield return null;
        }
 
        em.SetVelocity(Vector2.zero);
        currentState = em.player != null ? State.Hovering : State.Patrolling;
    }
 
    // ── Gizmos ────────────────────────────────────────────────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(transform.position.x - 3f, retreatHeight),
                        new Vector3(transform.position.x + 3f, retreatHeight));
    }
}
