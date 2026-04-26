using UnityEngine;
using System.Collections;
using TMPro;

public class DamageItem : MonoBehaviour
{
    private bool isActivated = false;
    
    [SerializeField] int damageAmount = 2;
    [SerializeField] float range = 1f;
    [SerializeField] LayerMask playerLayer;
    public AudioClip pickUpSound;
    
    [TextArea(2, 5)]
    [SerializeField] private string message = "Enter message";
    [SerializeField] private TextMeshProUGUI messageText;
    


    private void Update()
    {
        DamageIncrease();
    }

    private void DamageIncrease()
    {
        if (isActivated) return;
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, range, playerLayer);
        
        foreach (Collider2D player in playersInRange)
        {
            player.GetComponent<PlayerManager>().damageAmount += damageAmount;
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
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
