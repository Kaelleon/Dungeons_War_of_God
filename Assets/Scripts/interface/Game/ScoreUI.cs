using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private ArcadeGameLoop gameLoop;
    [SerializeField] private TMP_Text scoreText;       // solo el número
    [SerializeField] private TMP_Text gainText;        // "+300"
    [SerializeField] private RectTransform punchRoot;  // lo que "late" al sumar

    [SerializeField] private int minGainToShow = 50;   // ignora los +10 por segundo
    [SerializeField] private float gainDuration = 0.9f;
    [SerializeField] private float gainRiseSpeed = 40f;

    private int targetScore;
    private float displayedScore;
    private int pendingGain;
    private float gainTimer, punch;
    private Vector2 gainStartPos;

    private void Awake()
    {
        gainStartPos = gainText.rectTransform.anchoredPosition;
        gainText.alpha = 0f;
    }

    private void OnEnable() => gameLoop.OnScoreUpdated += HandleScore;
    private void OnDisable() => gameLoop.OnScoreUpdated -= HandleScore;

    private void HandleScore(int score)
    {
        int delta = score - targetScore;
        targetScore = score;

        if (delta < 0) { displayedScore = score; return; } // reinicio de partida
        if (delta < minGainToShow) return;

        // Si el popup anterior sigue visible, encadena los puntos
        pendingGain = gainTimer > 0f ? pendingGain + delta : delta;
        gainTimer = gainDuration;
        gainText.text = "+" + pendingGain.ToString("N0");
        punch = 1f;
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;

        // El número "corre" hacia el puntaje real
        float step = Mathf.Max(200f, (targetScore - displayedScore) * 5f) * dt;
        displayedScore = Mathf.MoveTowards(displayedScore, targetScore, step);
        scoreText.text = Mathf.RoundToInt(displayedScore).ToString("D7");

        punch = Mathf.MoveTowards(punch, 0f, 5f * dt);
        punchRoot.localScale = Vector3.one * (1f + punch * 0.25f);

        if (gainTimer > 0f)
        {
            gainTimer -= dt;
            float k = 1f - Mathf.Clamp01(gainTimer / gainDuration); // 0 → 1
            gainText.alpha = 1f - k * k;
            gainText.rectTransform.anchoredPosition = gainStartPos + Vector2.up * (k * gainRiseSpeed);
        }
        else gainText.alpha = 0f;
    }
}