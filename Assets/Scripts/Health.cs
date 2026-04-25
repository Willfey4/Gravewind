using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Health : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    public float deathDuration = 0.25f;



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
        else if (gameObject.CompareTag("Player") || gameObject.CompareTag("Enemy"))
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
            gameObject.GetComponent<Animator>().SetTrigger("IsHit");
        }
    }


    /* --------------- HELPERS FUNCTION --------------- */
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private IEnumerator deathRoutine()
    {
        yield return new WaitForSeconds(deathDuration);
    }
}
