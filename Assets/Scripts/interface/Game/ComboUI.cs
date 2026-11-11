using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ArcadeGameLoop gameLoop;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform animatedRoot;  // lo que escala y vibra
    [SerializeField] private TMP_Text multiplierText;     // "x5"
    [SerializeField] private TMP_Text labelText;          // "COMBO"
    [SerializeField] private Image timerFill;             // Filled / Horizontal

    [Header("Colores (de x2 a x10)")]
    [SerializeField]
    private Color[] tierColors =
    {
        new Color(1f, 0.92f, 0.3f), new Color(1f, 0.6f, 0.1f),
        new Color(1f, 0.25f, 0.15f), new Color(0.9f, 0.2f, 1f)
    };
    [SerializeField] private Color lowTimeColor = new Color(1f, 0.1f, 0.1f);
    [SerializeField] private Color brokenColor = new Color(0.55f, 0.55f, 0.55f);

    [Header("Animación")]
    [SerializeField] private float punchStrength = 0.6f;
    [SerializeField] private float lowTimeThreshold = 0.3f;
    [SerializeField] private float breakDuration = 0.6f;

    private Vector2 basePosition;
    private int displayedCombo = 1;
    private float punch, shake, breakTimer;

    private void Awake()
    {
        basePosition = animatedRoot.anchoredPosition;
        group.alpha = 0f;
    }

    private void OnEnable()
    {
        gameLoop.OnComboUpdated += HandleComboUpdated;
        gameLoop.OnComboHit += HandleComboHit;
    }

    private void OnDisable()
    {
        gameLoop.OnComboUpdated -= HandleComboUpdated;
        gameLoop.OnComboHit -= HandleComboHit;
    }

    private void HandleComboUpdated(int combo)
    {
        if (combo <= 1)
        {
            // Combo perdido (daño o tiempo agotado): se muestra apagado y vibrando
            if (displayedCombo > 1) { breakTimer = breakDuration; shake = 1f; }
            displayedCombo = 1;
            return;
        }

        breakTimer = 0f;
        displayedCombo = combo;
        multiplierText.text = "x" + combo;
        labelText.text = combo >= ArcadeGameLoop.MAX_COMBO ? "¡MÁXIMO!" : "COMBO";
    }

    private void HandleComboHit(int combo)
    {
        if (combo > 1) punch = 1f;
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        punch = Mathf.MoveTowards(punch, 0f, 5f * dt);
        shake = Mathf.MoveTowards(shake, 0f, 2.5f * dt);

        bool active = displayedCombo > 1;
        bool broken = breakTimer > 0f;
        float time01 = active ? gameLoop.ComboTimeNormalized : 0f;
        bool lowTime = active && time01 < lowTimeThreshold;

        Color tint = (broken || !active) ? brokenColor : TierColor(displayedCombo);
        multiplierText.color = tint;
        labelText.color = tint;

        timerFill.fillAmount = time01;
        timerFill.color = lowTime
            ? Color.Lerp(lowTimeColor, Color.white, Mathf.PingPong(Time.unscaledTime * 6f, 1f) * 0.6f)
            : tint;

        if (broken)
        {
            breakTimer -= dt;
            group.alpha = Mathf.Clamp01(breakTimer / breakDuration);
        }
        else
        {
            group.alpha = Mathf.MoveTowards(group.alpha, active ? 1f : 0f, 8f * dt);
        }

        float tierBoost = 1f + Mathf.Max(0, displayedCombo - 2) * 0.04f; // crece con el combo
        float amplitude = shake * 14f + (lowTime ? 2f : 0f);
        animatedRoot.localScale = Vector3.one * (tierBoost * (1f + punch * punchStrength));
        animatedRoot.anchoredPosition = basePosition + Random.insideUnitCircle * amplitude;
    }

    private Color TierColor(int combo)
    {
        if (tierColors.Length == 1) return tierColors[0];
        float t = Mathf.InverseLerp(2, ArcadeGameLoop.MAX_COMBO, combo) * (tierColors.Length - 1);
        int i = Mathf.Min((int)t, tierColors.Length - 2);
        return Color.Lerp(tierColors[i], tierColors[i + 1], t - i);
    }
} 