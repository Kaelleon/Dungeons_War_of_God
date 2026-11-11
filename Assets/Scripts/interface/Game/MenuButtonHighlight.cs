using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Resalta el botón cuando está seleccionado (teclado o mouse).
[RequireComponent(typeof(Button))]
public class MenuButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [Header("Referencias")]
    [SerializeField] private Image background;   // la Image del botón
    [SerializeField] private TMP_Text label;

    [Header("Colores")]
    [SerializeField] private Color normalBackground = new Color(0.12f, 0.12f, 0.16f, 0.9f);
    [SerializeField] private Color selectedBackground = new Color(1f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color normalText = Color.white;
    [SerializeField] private Color selectedText = new Color(0.1f, 0.1f, 0.1f, 1f);

    [Header("Animación")]
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float speed = 14f;

    private Button button;
    private bool isSelected;
    private Vector3 baseScale;
    private float blend; // 0 = normal, 1 = seleccionado

    private void Awake()
    {
        button = GetComponent<Button>();
        if (background == null) background = GetComponent<Image>();
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        isSelected = EventSystem.current != null &&
                     EventSystem.current.currentSelectedGameObject == gameObject;
        blend = isSelected ? 1f : 0f;
        Apply();
    }

    private void Update()
    {
        blend = Mathf.MoveTowards(blend, isSelected ? 1f : 0f, speed * Time.unscaledDeltaTime);
        Apply();
    }

    private void Apply()
    {
        background.color = Color.Lerp(normalBackground, selectedBackground, blend);
        label.color = Color.Lerp(normalText, selectedText, blend);
        transform.localScale = baseScale * Mathf.Lerp(1f, selectedScale, blend);
    }

    public void OnSelect(BaseEventData eventData) => isSelected = true;
    public void OnDeselect(BaseEventData eventData) => isSelected = false;

    // El mouse también selecciona, así hay una sola selección
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.IsInteractable()) EventSystem.current.SetSelectedGameObject(gameObject);
    }
}