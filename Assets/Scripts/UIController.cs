using UnityEngine;
using TMPro;
public class UIController : MonoBehaviour
{
    public GameObject player;

    [Header("Player Health")]
    public TextMeshProUGUI healthBar;

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
    }
}
