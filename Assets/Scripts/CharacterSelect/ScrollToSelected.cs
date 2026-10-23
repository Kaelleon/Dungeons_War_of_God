using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollToSelected : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    private GameObject lastChecked;

    private void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || selected == lastChecked) return;

        lastChecked = selected;
        if (!selected.transform.IsChildOf(scrollRect.content)) return;

        ScrollTo((RectTransform)selected.transform);
    }

    private void ScrollTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;
        float scrollable = contentHeight - viewportHeight;
        if (scrollable <= 0f) return; // Todo cabe, no hay que desplazar

        // Posición del objetivo medida desde el borde superior del content
        Vector3 worldCenter = target.TransformPoint(target.rect.center);
        Vector3 local = content.InverseTransformPoint(worldCenter);
        float distanceFromTop = content.rect.yMax - local.y;

        float targetTop = distanceFromTop - target.rect.height * 0.5f;
        float targetBottom = distanceFromTop + target.rect.height * 0.5f;

        float currentTop = (1f - scrollRect.verticalNormalizedPosition) * scrollable;
        float currentBottom = currentTop + viewportHeight;

        float newTop;
        if (targetTop < currentTop) newTop = targetTop;
        else if (targetBottom > currentBottom) newTop = targetBottom - viewportHeight;
        else return; // Ya está visible

        scrollRect.verticalNormalizedPosition = 1f - Mathf.Clamp01(newTop / scrollable);
    }
}