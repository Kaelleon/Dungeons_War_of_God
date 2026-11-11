using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthFill;               // Image con Image Type = Filled
    public TextMeshProUGUI healthText;


    private void OnEnable() => playerHealth.OnHealthChanged += UpdateHealth;
    private void OnDisable() => playerHealth.OnHealthChanged -= UpdateHealth;

    private void UpdateHealth(int current, int max)
    {
        healthFill.fillAmount = (float)current / max;
        healthText.text = current + " / " + max;
    }
}
