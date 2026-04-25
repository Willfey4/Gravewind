using UnityEngine;

public class DamageItem : MonoBehaviour
{
    [SerializeField] int damageAmount = 2;
    [SerializeField] float range = 1f;
    [SerializeField] LayerMask playerLayer;

    private void Update()
    {
        DamageIncrease();
    }

    private void DamageIncrease()
    {
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, range, playerLayer);
        foreach (Collider2D player in playersInRange)
        {
            player.GetComponent<PlayerManager>().damageAmount += damageAmount;
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
