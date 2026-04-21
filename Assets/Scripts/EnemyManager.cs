using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int damageAmount = 1;
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with " + other.gameObject.name);
            other.gameObject.GetComponent<Health>().TakeDamage(damageAmount);
        }
    }
}
