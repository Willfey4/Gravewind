using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    private bool isDead = false;
    private bool movementEnabled = true;
    private bool isHurt = false;

 
    [Header("Player Attack")]
    public int damageAmount = 1;
    [SerializeField] Vector2 knockbackForce = new Vector2(4f, 3f);
    public Transform attackHitbox;
    
 
    [Header("Attack Cooldown")]
    [SerializeField] float attackCooldown = 0.4f;   // minimum time between attacks
    private float attackTimer = 0f;
    private bool isFacingRight = true;   // kept in sync by PlayerInput

    [Header("Abilities")]
    [SerializeField] bool wallJumpUnlocked = false;
 

    [Header("Hurt State")]
    [SerializeField] float hurtDuration = 1f;

    [Header("Sound Effects")]
    public AudioClip[] attackSounds;
    public AudioClip hurtSound;
    public AudioClip[] footstepSounds;
    public AudioClip deathSound;
    public AudioClip[] jumpSounds;


    private void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }
 

 
    public void SetFacingRight(bool facingRight) // Fixest attack hitbox direction when player changes facing direction
    {
        if (isFacingRight == facingRight) return; // no change, skip
        isFacingRight = facingRight;
 
        // Flip the hitbox's local X position to the opposite side
        Vector3 pos = attackHitbox.localPosition;
        pos.x = Mathf.Abs(pos.x) * (facingRight ? 1f : -1f);
        attackHitbox.localPosition = pos;
    }



 
    /* --------------- ATTACK FUNCTIONS --------------- */
    public bool CanAttack() // Determines if the player can attack based on the cooldown timer
    {
        return attackTimer <= 0f;
    }
 
    public void hit()
    {
        // Reset cooldown on each attack
        attackTimer = attackCooldown;
        AudioManager.Instance.PlayRandomAudioClip(attackSounds, transform);
 
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackHitbox.position, attackHitbox.localScale, 0f);
 
        foreach (Collider2D enemy in hitEnemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemy.GetComponent<Health>().TakeDamage(damageAmount);

                float kbDir = enemy.transform.position.x > transform.position.x ? 1f : -1f;
                enemyRb.linearVelocity = Vector2.zero;
                enemyRb.AddForce(new Vector2(knockbackForce.x * kbDir, knockbackForce.y), ForceMode2D.Impulse);

                // Pause the enemy's AI so it doesn't immediately walk back over the knockback
                EnemyManager em = enemy.GetComponent<EnemyManager>();
                if (em != null) em.TriggerHurt();
            }
            break;
            
        }
    }


    /* --------------- ABILITIES --------------- */
    public void UnlockWallJump()
    {
        wallJumpUnlocked = true;
        Debug.Log("Wall jump unlocked.");
    }
 
    public void LockWallJump()
    {
        wallJumpUnlocked = false;
    }
 
    public bool CanWallJump()
    {
        return wallJumpUnlocked;
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
        movementEnabled = false; // Disable player movement when hurt
        yield return new WaitForSeconds(hurtDuration);
        movementEnabled = true; // Re-enable movement after hurt duration
        isHurt = false;
    }
 
    

    /* --------------- HELPERS FUNCTIONS --------------- */
 
    public void setIsDead(bool value)
    {
        isDead = value;
    }
 
    public bool getIsDead()
    {
        return isDead;
    }
 
    public void setMovementEnabled(bool value)
    {
        movementEnabled = value;
    }
 
    public bool getMovementEnabled()
    {
        return movementEnabled;
    }

    public bool GetIsHurt()
    {
        return isHurt;
    }
}
