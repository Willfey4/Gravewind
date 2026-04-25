using UnityEngine;
using TMPro;

public class Sign : MonoBehaviour
{
    [TextArea(2, 5)]
    [SerializeField] private string message = "Enter message";
    [SerializeField] private TextMeshProUGUI messageText;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            messageText.text = message;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            messageText.text = "";
    }
}
