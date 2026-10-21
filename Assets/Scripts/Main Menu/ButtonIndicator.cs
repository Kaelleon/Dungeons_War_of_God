using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonIndicator : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [Header("Referencias")]
    [SerializeField] private TMP_Text texto;   // el texto del botón
    [SerializeField] private Graphic flecha;   // la flecha (sirve Image o TMP)

    [Header("Colores")]
    [SerializeField] private Color textoNormal = Color.white;
    [SerializeField] private Color textoSeleccionado = Color.yellow;
    [SerializeField] private Color flechaColor = Color.yellow;

    private void OnEnable()
    {
        // En vez de apagar la flecha a ciegas, revisamos si ya estamos seleccionados
        bool yaSeleccionado = EventSystem.current != null &&
                              EventSystem.current.currentSelectedGameObject == gameObject;
        Aplicar(yaSeleccionado);
    }

    public void OnSelect(BaseEventData eventData) => Aplicar(true);
    public void OnDeselect(BaseEventData eventData) => Aplicar(false);

    // El mouse también selecciona, para que haya una sola selección
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private void Aplicar(bool seleccionado)
    {
        if (texto != null)
            texto.color = seleccionado ? textoSeleccionado : textoNormal;

        if (flecha != null)
        {
            flecha.color = flechaColor;
            flecha.gameObject.SetActive(seleccionado);
        }
    }
}