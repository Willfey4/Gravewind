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
        else if (gameObject.CompareTag("Player") || gameObject.CompareTag("Enemy") || gameObject.CompareTag("Boss"))
        {
            gameObject.GetComponent<Animator>().SetTrigger("IsHit");
            if (gameObject.CompareTag("Player")) AudioManager.Instance.PlayAudioClip(gameObject.GetComponent<PlayerManager>().hurtSound, transform);
            else if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Boss")) AudioManager.Instance.PlayAudioClip(gameObject.GetComponent<EnemyManager>().hurtSound, transform);
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
            AudioManager.Instance.PlayAudioClip(gameObject.GetComponent<PlayerManager>().deathSound, transform);
            gameObject.GetComponent<PlayerManager>().setIsDead(true);
        }

        else if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Boss"))
        {
            Debug.Log("Enemy Died");
            AudioManager.Instance.PlayAudioClip(gameObject.GetComponent<EnemyManager>().deathSound, transform, .5f);
            gameObject.GetComponent<Animator>().SetTrigger("IsHit");
            if (gameObject.CompareTag("Boss"))
            {
                StartCoroutine(bossDeathRoutine());
            }
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


    private IEnumerator bossDeathRoutine()
    {
        yield return new WaitForSeconds(3f);
        Time.timeScale = 0f;
        PlayerManager pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        pm.pauseMenuUI.SetActive(true);
        pm.messageText.text = "Game completed! Congratulations!";
    }
}
