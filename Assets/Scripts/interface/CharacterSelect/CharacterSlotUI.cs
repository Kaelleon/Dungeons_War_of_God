using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject selectionFrame;
    [SerializeField] private GameObject lockedFrame;   // NUEVO: marco fijo mientras está bloqueado
    [SerializeField] private Button button;
    [SerializeField] private Color dimmedColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    private CharacterData data;
    private Action<CharacterData> onHighlighted;
    private Action<CharacterSlotUI> onPressed;

    public CharacterData Data => data;

    public void Setup(CharacterData characterData,
                      Action<CharacterData> highlightedCallback,
                      Action<CharacterSlotUI> pressedCallback)
    {
        data = characterData;
        onHighlighted = highlightedCallback;
        onPressed = pressedCallback;

        portraitImage.sprite = data.portrait;
        selectionFrame.SetActive(false);
        lockedFrame.SetActive(false);

        // Click o Enter (Submit) disparan onClick
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onPressed?.Invoke(this));
    }

    // Mientras hay un personaje bloqueado, los demás no se pueden elegir
    public void SetLockState(bool isLockedSlot, bool anyLocked)
    {
        lockedFrame.SetActive(isLockedSlot);
        button.interactable = !anyLocked || isLockedSlot;
        portraitImage.color = (anyLocked && !isLockedSlot) ? dimmedColor : Color.white;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!button.interactable) return;
        selectionFrame.SetActive(true);
        onHighlighted?.Invoke(data);
    }

    public void OnDeselect(BaseEventData eventData) => selectionFrame.SetActive(false);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}