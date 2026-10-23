using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class AbilitySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectionFrame;

    private Selectable selectable;
    private AbilityData ability;
    private AbilityTooltip tooltip;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
    }

    public void Setup(AbilityData abilityData, AbilityTooltip tooltipRef)
    {
        if (selectable == null) selectable = GetComponent<Selectable>();

        ability = abilityData;
        tooltip = tooltipRef;

        bool hasAbility = ability != null;
        iconImage.enabled = hasAbility && ability.icon != null;
        iconImage.sprite = hasAbility ? ability.icon : null;
        selectionFrame.SetActive(false);

        // Un slot vacío no se puede seleccionar
        selectable.interactable = hasAbility;
    }

    public void OnSelect(BaseEventData eventData)
    {
        selectionFrame.SetActive(true);
        if (ability != null) tooltip.Show(ability, (RectTransform)transform);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selectionFrame.SetActive(false);
        tooltip.Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ability != null) EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Al sacar el mouse ocultamos el recuadro
        tooltip.Hide();
    }
}