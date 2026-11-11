using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotHUD : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownOverlay;  // Filled, Radial 360, negro semitransparente
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private TMP_Text keyLabel;

    [SerializeField] private Color readyColor = Color.white;
    [SerializeField] private Color cooldownColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    private float punch;          // >0 al quedar lista, <0 al usarla
    private bool wasOnCooldown;

    public void Bind(AbilityData data, string key)
    {
        gameObject.SetActive(data != null); // sin habilidad, el slot se oculta
        if (data == null) return;

        icon.sprite = data.icon;
        icon.enabled = data.icon != null;
        keyLabel.text = key;
        wasOnCooldown = false;
        punch = 0f;
    }

    public void PlayUsed() => punch = -1f; // pequeño "hundimiento" al usarla

    public void Tick(float remaining, float cooldown)
    {
        bool onCooldown = remaining > 0f;

        if (wasOnCooldown && !onCooldown) punch = 1f; // vuelve a estar lista: "pop"
        wasOnCooldown = onCooldown;

        icon.color = onCooldown ? cooldownColor : readyColor; // gris en cooldown
        cooldownOverlay.fillAmount = onCooldown ? remaining / Mathf.Max(0.01f, cooldown) : 0f;

        if (!onCooldown) cooldownText.text = "";
        else if (remaining >= 1f) cooldownText.text = Mathf.CeilToInt(remaining).ToString();
        else cooldownText.text = remaining.ToString("0.0", CultureInfo.InvariantCulture);

        punch = Mathf.MoveTowards(punch, 0f, 6f * Time.unscaledDeltaTime);
        transform.localScale = Vector3.one * (1f + punch * 0.2f);
    }
}