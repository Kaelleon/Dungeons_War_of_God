using System.Collections;
using TMPro;
using UnityEngine;

public class GameFlowUI : MonoBehaviour
{
    [Header("Intro")]
    [SerializeField] private CanvasGroup introGroup;
    [SerializeField] private TextMeshProUGUI introTitleText;  
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Pausa")]
    [SerializeField] private GameObject pausePanel;

    [Header("Game Over")]
    [SerializeField] private CanvasGroup gameOverGroup;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private GameObject newRecordLabel;
    [SerializeField] private TextMeshProUGUI statsText;

    private void Awake()
    {
        introGroup.gameObject.SetActive(true);
        pausePanel.SetActive(false);
        gameOverGroup.gameObject.SetActive(false);
    }

    public IEnumerator PlayIntro(CharacterData character, int seconds)
    {
        introTitleText.text = character.characterName.ToUpper() + " · " + character.characterClass.ToUpper();

        for (int i = seconds; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return Pulse(countdownText.rectTransform, 0.9f);
        }

        countdownText.text = "¡A LUCHAR!";
        yield return Pulse(countdownText.rectTransform, 0.6f);
        yield return Fade(introGroup, 1f, 0f, 0.3f);
        introGroup.gameObject.SetActive(false);
    }

    public void ShowPause(bool show) => pausePanel.SetActive(show); //MenuController selecciona el primer botón

    public void ShowGameOver(GameResult r, int previousBest, bool isNewRecord)
    {
        finalScoreText.text = "Puntaje: " + r.score.ToString("N0");
        bestScoreText.text = "MEJOR: " + Mathf.Max(previousBest, r.score).ToString("N0");
        newRecordLabel.SetActive(isNewRecord);
        statsText.text = "Tiempo   " + FormatTime(r.timeSurvived) + "\n"
                       + "Bajas   " + r.kills + "\n"
                       + "Jefes derrotados   " + r.bossesDefeated + "\n"
                       + "Combo máximo   x" + r.maxCombo;

        gameOverGroup.gameObject.SetActive(true);
        StartCoroutine(GameOverFadeIn());
    }

    private IEnumerator GameOverFadeIn()
    {
        gameOverGroup.interactable = false; //evita saltarse la pantalla machacando teclas
        yield return Fade(gameOverGroup, 0f, 1f, 0.5f);
        yield return new WaitForSecondsRealtime(0.4f);
        gameOverGroup.interactable = true;
    }

    //todo usa tiempo sin escala para funcionar con Time.timeScale = 0
    private IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        group.alpha = to;
    }

    private IEnumerator Pulse(RectTransform target, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            target.localScale = Vector3.one * Mathf.Lerp(1.6f, 1f, 1f - (1f - k) * (1f - k));
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    private static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
    }
}