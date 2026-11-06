using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private GameObject selectionFrame;
    [SerializeField] private Button button;

    private CharacterData data;
    private Action<CharacterData> onHighlighted;
    private Action<CharacterData> onConfirmed;

    public void Setup(CharacterData characterData,
                      Action<CharacterData> highlightedCallback,
                      Action<CharacterData> confirmedCallback)
    {
        data = characterData;
        onHighlighted = highlightedCallback;
        onConfirmed = confirmedCallback;

        portraitImage.sprite = data.portrait;
        selectionFrame.SetActive(false);

        // Click o Enter (Submit) disparan onClick
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onConfirmed?.Invoke(data));
    }

    public void OnSelect(BaseEventData eventData)
    {
        selectionFrame.SetActive(true);
        onHighlighted?.Invoke(data);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selectionFrame.SetActive(false);
    }

    // El mouse también selecciona, así hay una sola selección
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}