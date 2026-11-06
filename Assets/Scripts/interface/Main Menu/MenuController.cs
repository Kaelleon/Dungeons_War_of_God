using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button primerBoton;

    private GameObject ultimoSeleccionado;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(primerBoton.gameObject);
        ultimoSeleccionado = primerBoton.gameObject;
    }

    private void Update()
    {
        var actual = EventSystem.current.currentSelectedGameObject;

        if (actual != null)
        {
            // Guardamos siempre el último válido
            ultimoSeleccionado = actual;
        }
        else if (ultimoSeleccionado != null && ultimoSeleccionado.activeInHierarchy)
        {
            // Si se perdió la selección (clic en vacío), la recuperamos
            EventSystem.current.SetSelectedGameObject(ultimoSeleccionado);
        }
    }
}
