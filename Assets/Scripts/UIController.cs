using UnityEngine;
using TMPro;
public class UIController : MonoBehaviour
{
    public GameObject player;

    [Header("Player Health")]
    public TextMeshProUGUI healthBar;
    public RectTransform healthBarUI;

    private void Start() {
        if(player == null)
        {
            Debug.LogError("Player reference is not set in UIController.");
        }

        if(healthBar == null)
        {
            Debug.LogError("HealthBar reference is not set in UIController.");
        }
    }
    private void Update() {
        healthBar.text = $"Health: {player.GetComponent<Health>().GetCurrentHealth()}";
        healthBarUI.sizeDelta = new Vector2((player.GetComponent<Health>().GetCurrentHealth() * 20) - 3, healthBarUI.sizeDelta.y); // Adjust the multiplier as needed for visual scaling
    }
}
