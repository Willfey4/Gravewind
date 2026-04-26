using UnityEngine;
using System.Collections;
using TMPro;

public class Heal : MonoBehaviour
{
    [SerializeField] int healAmount = 2;
    [SerializeField] float healRange = 1f;
    [SerializeField] LayerMask playerLayer;
    public AudioClip pickUpSound;

    [TextArea(2, 5)]
    [SerializeField] private string message = "Enter message";
    [SerializeField] private TextMeshProUGUI messageText;
    private bool isActivated = false;

    private void Update()
    {
        HealPlayer();
    }

    private void HealPlayer()
    {
        if (isActivated) return;
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, healRange, playerLayer);
        foreach (Collider2D player in playersInRange)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);
            }
            AudioManager.Instance.PlayAudioClip(pickUpSound, transform, .2f);
            StartCoroutine(DelayedForMessage());
        }
    }

    private IEnumerator DelayedForMessage()
    {
        messageText.text = message;
        GetComponent<SpriteRenderer>().enabled = false;
        isActivated = true;
        yield return new WaitForSeconds(2f);
        messageText.text = "";
        Destroy(gameObject); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, healRange);
    }
}
