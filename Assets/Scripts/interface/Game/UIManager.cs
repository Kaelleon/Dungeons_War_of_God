using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public ArcadeGameLoop gameLoop;
    public PlayerHealth playerHealth;
    public Image healthFill;               // Image con Image Type = Filled
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI comboText;


    private void OnEnable()
    {
        gameLoop.OnScoreUpdated += ActualizarPantalla;
        playerHealth.OnHealthChanged += ActualizarVida;
        gameLoop.OnComboUpdated += ActualizarCombo;
    }

    private void OnDisable()
    {
        gameLoop.OnScoreUpdated -= ActualizarPantalla;
        playerHealth.OnHealthChanged -= ActualizarVida;
        gameLoop.OnComboUpdated -= ActualizarCombo;
    }

    private void ActualizarPantalla(int nuevoPuntaje)
    {
        scoreText.text = "PUNTOS: " + nuevoPuntaje;
    }

    private void ActualizarVida(int current, int max)
    {
        healthFill.fillAmount = (float)current / max;
        healthText.text = current + " / " + max;
    }

    private void ActualizarCombo(int combo)
    {
        comboText.text = combo > 1 ? "COMBO x" + combo : "";
    }
}
