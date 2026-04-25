using System.Collections;
using UnityEngine;
 
public class MeleeEnemy : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] float chaseSpeed = 3.5f;
    
    [Header("Melee Attack")]
    [SerializeField] Transform attackHitbox;
    [SerializeField] Vector2 attackHitboxSize = new Vector2(0.8f, 0.8f);
    [SerializeField] int attackDamage = 2;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] float attackRange = 0.9f;
 
    [Header("Attack Knockback")]
    [SerializeField] Vector2 knockbackForce = new Vector2(4f, 3f);
 
 
    // ── Private ────────────────────────────────────────────────────────────
    private enum State { Patrolling, Chasing, Attacking }
    private State currentState = State.Patrolling;
 
    private EnemyManager em;
    private Animator anim;
    private float attackTimer = 0f;
 
    private void Awake()
    {
        em = GetComponent<EnemyManager>();
        anim = GetComponent<Animator>();
    }
 
    private void Update()
    {
        if (em.isDead) return;
 
        attackTimer -= Time.deltaTime;
 
        // If hurt, flip to face the player (hit from behind) then resume
        if (em.isHurt)
        {
            em.StopHorizontal();
            anim.SetBool("IsWalking", false);
            if (em.player != null) 
                em.FacePlayer();
            return;
        }

        switch (currentState)
        {
            case State.Patrolling: UpdatePatrol(); break;
            case State.Chasing: UpdateChase(); break;
            case State.Attacking: UpdateAttacking(); break;
        }
    }
 
    /* --------------- Enemy State Updates --------------- */
 
    private void UpdatePatrol()
    {
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsWalking", true);
        em.Patrol();
 
        if (em.CheckDetectionBox())
        {
            currentState = State.Chasing;
        }
    }


    private void UpdateChase()
    {
        em.CheckDetectionBox();
 
        if (em.player == null)
        {
            currentState = State.Patrolling;
            anim.SetBool("IsWalking", false);
            return;
        }

        anim.SetBool("IsRunning", true);
 
        float dist = Vector2.Distance(transform.position, em.player.position);
 
        if (dist <= attackRange && attackTimer <= 0f)
        {
            currentState = State.Attacking;
            return;
        }
 
        float dir = em.player.position.x > transform.position.x ? 1f : -1f;
        em.SetVelocityX(dir * chaseSpeed);
        em.FaceDirection(dir);
        anim.SetBool("IsWalking", true);
    }
 

    private void UpdateAttacking()
    {
        em.StopHorizontal();
        anim.SetBool("IsRunning", false);
        anim.SetBool("IsWalking", false);
 
        if (attackTimer > 0f) return;
 
        StartCoroutine(PerformAttack());
    }
 
    private IEnumerator PerformAttack()
    {
        attackTimer = attackCooldown;
        anim.SetBool("IsAttacking", true);
 
        yield return new WaitForSeconds(2f);
 
        if (attackHitbox != null)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(attackHitbox.position, attackHitboxSize, 0f);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<Health>().TakeDamage(attackDamage);
 
                    Rigidbody2D playerRb = hit.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        float kbDir = hit.transform.position.x > transform.position.x ? 1f : -1f;
                        playerRb.linearVelocity = Vector2.zero;
                        playerRb.AddForce(new Vector2(knockbackForce.x * kbDir, knockbackForce.y), ForceMode2D.Impulse);
                    }
 
                    PlayerManager pm = hit.GetComponent<PlayerManager>();
                    if (pm != null) pm.TriggerHurt();
 
                    break;
                }
            }
        }
 
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("IsAttacking", false);
 
        currentState = em.player != null ? State.Chasing : State.Patrolling;
    }
 
    /* --------------- Attack Hitbox Gizmos --------------- */
    private void OnDrawGizmosSelected()
    {
        if (attackHitbox != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Gizmos.DrawWireCube(attackHitbox.position, attackHitboxSize);
        }
    }
}