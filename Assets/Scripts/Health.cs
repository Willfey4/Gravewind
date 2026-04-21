using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            Die();
        }
        else if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<Animator>().SetTrigger("IsHit");
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    void Die()
    {
        
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Player Died");
            gameObject.GetComponent<Animator>().SetBool("IsAlive", false);
            gameObject.GetComponent<PlayerInput>().enabled = false;
            gameObject.GetComponent<PlayerManager>().setIsDead(true);
        }

        else if (gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy Died");
            Destroy(gameObject);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }




}
