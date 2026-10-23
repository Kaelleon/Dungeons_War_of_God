using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public ArcadeGameLoop gameLoop;

    private void OnEnable()
    {
        gameLoop.OnScoreUpdated += ActualizarPantalla;
    }

    private void OnDisable()
    {
        gameLoop.OnScoreUpdated -= ActualizarPantalla;
    }

    private void ActualizarPantalla(int nuevoPuntaje)
    {
        scoreText.text = "PUNTOS: " + nuevoPuntaje;
    }
}
