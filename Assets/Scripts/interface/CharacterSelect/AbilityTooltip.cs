using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTooltip : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private RectTransform canvasRect;

    private RectTransform rect;

    private RectTransform Rect
    {
        get
        {
            if (rect == null) rect = (RectTransform)transform;
            return rect;
        }
    }

    public void Show(AbilityData ability, RectTransform anchor)
    {
        titleText.text = ability.abilityName;
        descriptionText.text = ability.description;
        gameObject.SetActive(true);

        // Colocamos el recuadro justo debajo del slot de la habilidad
        Vector3[] corners = new Vector3[4];
        anchor.GetWorldCorners(corners); // 0 = abajo-izq, 3 = abajo-der
        Vector3 bottomCenter = (corners[0] + corners[3]) * 0.5f;

        Rect.pivot = new Vector2(0.5f, 1f);
        Rect.position = bottomCenter + Vector3.down * 10f;

        // Recalculamos el tamaño según el texto y evitamos que se salga de la pantalla
        LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
        ClampToCanvas();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ClampToCanvas()
    {
        Vector3[] tip = new Vector3[4];
        Vector3[] bounds = new Vector3[4];
        Rect.GetWorldCorners(tip);
        canvasRect.GetWorldCorners(bounds);

        Vector3 offset = Vector3.zero;
        if (tip[2].x > bounds[2].x) offset.x = bounds[2].x - tip[2].x;
        if (tip[0].x < bounds[0].x) offset.x = bounds[0].x - tip[0].x;
        if (tip[0].y < bounds[0].y) offset.y = bounds[0].y - tip[0].y;

        Rect.position += offset;
    }
}