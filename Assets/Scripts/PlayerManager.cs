using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private bool isDead = false;
    private bool movementEnabled = true;
    
    [Header("Player Attack")]
    public int damageAmount = 1;
    public Transform attackHitbox;

    private void FixedUpdate() {
        if (isDead == false)
        {
            return;
        }
        else
        {
            return;
        }
    }

    public void hit()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackHitbox.position, attackHitbox.localScale, 0f);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Debug.Log("Hit " + enemy.name);
                enemy.GetComponent<Health>().TakeDamage(damageAmount);
                Debug.Log("Health of " + enemy.name + ": " + enemy.GetComponent<Health>().GetCurrentHealth());
            }
        }
    }

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
}
