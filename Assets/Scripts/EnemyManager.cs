using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [Header("Contact Damage and Knockback")]
    public int damageAmount = 1;
    [SerializeField] Vector2 knockbackForce = new Vector2(4f, 3f);
 

    [Header("Patrol")]
    [Tooltip("Leave both null to make the enemy stand still.")]
    public Transform leftPatrolPoint;
    public Transform rightPatrolPoint;
    public float patrolSpeed = 2f;
 

    [Header("Detection")]
    public float detectionRange = 5f;
    public float detectionHeight = 1.5f; 
    public float loseAggroRange = 8f;
    public LayerMask playerLayer;


    [Header("Hurt State")]
    [SerializeField] float hurtDuration = 0.25f;
 

    
    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public Transform player;      // non-null when player is detected
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isHurt = false;
 
    
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Health health;

 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
    }
 
    private void Update()
    {
        if (isDead) return;
 
        if (health.GetCurrentHealth() <= 0)
        {
            HandleDeath();
            return;
        }
    }


 
    /* --------------- Patrol --------------- */
    public void Patrol()
    {
        if (leftPatrolPoint == null || rightPatrolPoint == null) return;
 
        if (facingRight)
        {
            SetVelocityX(patrolSpeed);
            if (transform.position.x >= rightPatrolPoint.position.x)
                Flip();
        }
        else
        {
            SetVelocityX(-patrolSpeed);
            if (transform.position.x <= leftPatrolPoint.position.x)
                Flip();
        }
    }

 

    /* --------------- Detection --------------- */   
    public bool CheckDetectionBox()
    {
        Vector2 origin = transform.position;
        Vector2 size   = new Vector2(detectionRange, detectionHeight * 2f);
        Vector2 offset = new Vector2((facingRight ? 1f : -1f) * detectionRange * 0.5f, 0f);
 
        Collider2D hit = Physics2D.OverlapBox(origin + offset, size, 0f, playerLayer);
 
        if (hit != null && hit.CompareTag("Player"))
        {
            player = hit.transform;
            return true;
        }
 
        // Drop aggro if player moved out of range
        if (player != null && Vector2.Distance(transform.position, player.position) > loseAggroRange)
            player = null;
 
        return player != null;
    }
 

    public bool CheckDetectionCircle() // Flying Enemies
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
 
        if (hit != null)
        {
            player = hit.transform;
            return true;
        }
 
        if (player != null && Vector2.Distance(transform.position, player.position) > loseAggroRange)
            player = null;
 
        return player != null;
    }



    /* --------------- HURT STATE --------------- */
    public void TriggerHurt()
    {
        if (isHurt) return;
        StartCoroutine(HurtRoutine());
    }
 
    private IEnumerator HurtRoutine()
    {
        isHurt = true;
        yield return new WaitForSeconds(hurtDuration);
        isHurt = false;
    }
 


    /* --------------- Flip Facing --------------- */
    public void Flip()
    {
        sprite.flipX = facingRight;
        facingRight = !facingRight;
    }
 
    public void FaceDirection(float dirX) 
    {
        bool shouldFaceRight = dirX > 0f;
        if (shouldFaceRight != facingRight) Flip();
    }
 
    public void FacePlayer()
    {
        if (player == null) return;
        FaceDirection(player.position.x > transform.position.x ? 1f : -1f);
    }
 



    /* --------------- Velocity Helpers --------------- */
    public void SetVelocityX(float x)
    {
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }
 
    
    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }
 
    public void StopHorizontal()
    {
        SetVelocityX(0f);
    }
 
    public Rigidbody2D GetRigidbody() => rb;
 



    /* --------------- Death --------------- */
    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;
 
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false; 
    }

    
 
    

    /* --------------- Damage on Contact --------------- */
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Health>().TakeDamage(damageAmount);

            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                float kbDir = other.gameObject.transform.position.x > transform.position.x ? 1f : -1f;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.AddForce(new Vector2(knockbackForce.x * kbDir, knockbackForce.y), ForceMode2D.Impulse);
            }
 
            // Trigger hurt window so the player's movement input doesn't cancel the knockback
            PlayerManager pm = other.gameObject.GetComponent<PlayerManager>();
            if (pm != null) pm.TriggerHurt();
        }
    }
 
    

    /* --------------- Patrol Gizmos --------------- */
    private void OnDrawGizmosSelected()
    {
        // Patrol bounds
        Gizmos.color = Color.cyan;
        if (leftPatrolPoint)  Gizmos.DrawSphere(leftPatrolPoint.position, 0.15f);
        if (rightPatrolPoint) Gizmos.DrawSphere(rightPatrolPoint.position, 0.15f);
 
        // Forward detection box
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Vector2 offset = new Vector2((facingRight ? 1f : -1f) * detectionRange * 0.5f, 0f);
        Gizmos.DrawCube((Vector2)transform.position + offset,
                        new Vector3(detectionRange, detectionHeight * 2f, 0f));
 
        // Lose aggro range
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawWireSphere(transform.position, loseAggroRange);
    }


    /* --------------- Helper Functions --------------- */

    public Rigidbody2D GetRigidbody2D() => rb;

}
