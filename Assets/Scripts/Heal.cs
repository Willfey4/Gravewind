using UnityEngine;

public class Heal : MonoBehaviour
{
    [SerializeField] int healAmount = 2;
    [SerializeField] float healRange = 1f;
    [SerializeField] LayerMask playerLayer;

    private void Update()
    {
        HealPlayer();
    }

    private void HealPlayer()
    {
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, healRange, playerLayer);
        foreach (Collider2D player in playersInRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }
             Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
