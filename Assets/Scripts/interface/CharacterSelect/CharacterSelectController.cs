using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterSelectController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private CharacterData[] characters;
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI")]
    [SerializeField] private CharacterSlotUI slotPrefab;
    [SerializeField] private Transform gridContent;
    [SerializeField] private CharacterInfoPanel infoPanel;

    private readonly List<CharacterSlotUI> slots = new List<CharacterSlotUI>();
    private GameObject lastSelected;
    private bool isLoading;

    private void Start()
    {
        BuildGrid();
        StartCoroutine(SelectFirstSlot());
    }

    private void BuildGrid()
    {
        foreach (CharacterData data in characters)
        {
            CharacterSlotUI slot = Instantiate(slotPrefab, gridContent);
            slot.Setup(data, OnCharacterHighlighted, OnCharacterConfirmed);
            slots.Add(slot);
        }
    }

    private IEnumerator SelectFirstSlot()
    {
        // Esperamos un frame para que el layout y el EventSystem estén listos
        yield return null;

        if (slots.Count == 0) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(slots[0].gameObject);
        lastSelected = slots[0].gameObject;
    }

    private void Update()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current != null)
            lastSelected = current;
        else if (lastSelected != null && lastSelected.activeInHierarchy)
            // Si se perdió la selección (click en vacío), la recuperamos
            EventSystem.current.SetSelectedGameObject(lastSelected);
    }

    private void OnCharacterHighlighted(CharacterData data)
    {
        infoPanel.Show(data);
    }

    private void OnCharacterConfirmed(CharacterData data)
    {
        if (isLoading) return;
        isLoading = true;

        GameSession.SelectedCharacter = data;
        SceneManager.LoadScene(gameSceneName);
    }
}