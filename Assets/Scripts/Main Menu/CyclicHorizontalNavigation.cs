using UnityEngine;
using UnityEngine.UI;

public class cyclicHorizontalNavigation : MonoBehaviour
{
    [SerializeField] private Button[] botones;

    private void Awake()
    {
        for (int i = 0; i < botones.Length; i++)
        {
            var nav = new Navigation { mode = Navigation.Mode.Explicit };

            // Izquierda: el botón anterior (el primero apunta al último)
            nav.selectOnLeft = botones[(i - 1 + botones.Length) % botones.Length];

            // Derecha: el botón siguiente (el último apunta al primero)
            nav.selectOnRight = botones[(i + 1) % botones.Length];

            botones[i].navigation = nav;
        }
    }
}