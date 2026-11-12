using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoPanel : MonoBehaviour
{
    [Header("Center")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image fullBodyImage;

    [Header("Abilities")]
    [SerializeField] private AbilitySlotUI[] abilitySlots;
    [SerializeField] private AbilityTooltip tooltip;

    [Header("Stats")]
    [SerializeField] private TMP_Text classText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text attackSpeedText;
    [SerializeField] private TMP_Text evasionText;

    [Header("Confirmación")]
    [SerializeField] private TMP_Text confirmText;

    private bool isLocked;
    private void Awake() => confirmText.gameObject.SetActive(false);

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        confirmText.gameObject.SetActive(locked);
    }

    private void Update()
    {
        if (!isLocked) return;
        //Parpadeo suave para que se note que hay que confirmar
        confirmText.alpha = Mathf.Lerp(0.55f, 1f, (Mathf.Sin(Time.unscaledTime * 5f) + 1f) * 0.5f);
    }

    public void Show(CharacterData data)
    {
        nameText.text = data.characterName;

        fullBodyImage.enabled = data.fullBody != null;
        fullBodyImage.sprite = data.fullBody;

        classText.text = "Clase: " + data.characterClass;
        healthText.text = "Salud: " + data.maxHealth + " HP";
        defenseText.text = "Defensa: +" + Format(data.defensePercent, "0") + "%";
        moveSpeedText.text = "Vel. Mov: " + Format(data.moveSpeedMultiplier, "0.0") + "x";
        attackText.text = "Ataque: " + data.attackPoints + " pts.";
        attackSpeedText.text = "Vel. Atq: " + Format(data.attackSpeedMultiplier, "0.0") + "x";
        evasionText.text = "Evasión: " + Format(data.evasionMultiplier, "0.0") + "x";

        tooltip.Hide();
        for (int i = 0; i < abilitySlots.Length; i++)
        {
            AbilityData ability = i < data.abilities.Length ? data.abilities[i] : null;
            abilitySlots[i].Setup(ability, tooltip);
        }
    }

    // InvariantCulture para que salga "0.8" y no "0,8" (el sistema en español usa coma)
    private string Format(float value, string format)
    {
        return value.ToString(format, CultureInfo.InvariantCulture);
    }
}