using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    [SerializeField] GameObject boss;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            boss.SetActive(true);
            Destroy(gameObject);
        }
    }
}
