using UnityEngine;

public class WallJumpUnlock : MonoBehaviour
{
    [SerializeField] float range = 1f;
    [SerializeField] LayerMask playerLayer;

    private void Update()
    {
        GainWallJump();
    }

    private void GainWallJump()
    {
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, range, playerLayer);
        foreach (Collider2D player in playersInRange)
        {
            player.GetComponent<PlayerManager>().UnlockWallJump();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
