using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    [Header("Confirmación")]
    [SerializeField] private float confirmDelay = 0.25f; // evita que un doble click bloquee y confirme a la vez

    private readonly List<CharacterSlotUI> slots = new List<CharacterSlotUI>();
    private GameObject lastSelected;
    private CharacterSlotUI lockedSlot;   // null = navegando libremente
    private float lockedAt;
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
            slot.Setup(data, OnCharacterHighlighted, OnCharacterPressed);
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

        // Escape suelta el personaje bloqueado
        if (lockedSlot != null && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Unlock();
    }

    private void OnCharacterHighlighted(CharacterData data)
    {
        if (lockedSlot != null) return; // bloqueado: el panel no cambia de personaje
        infoPanel.Show(data);
    }

    // Click o Enter sobre un slot
    private void OnCharacterPressed(CharacterSlotUI slot)
    {
        if (isLoading) return;

        if (lockedSlot == null)
        {
            Lock(slot);
        }
        else if (slot == lockedSlot && Time.unscaledTime - lockedAt >= confirmDelay)
        {
            Confirm(slot.Data);
        }
    }

    private void Lock(CharacterSlotUI slot)
    {
        lockedSlot = slot;
        lockedAt = Time.unscaledTime;

        infoPanel.Show(slot.Data);
        infoPanel.SetLocked(true);
        foreach (CharacterSlotUI s in slots) s.SetLockState(s == slot, true);

        EventSystem.current.SetSelectedGameObject(slot.gameObject);
    }

    private void Unlock()
    {
        CharacterSlotUI slot = lockedSlot;
        lockedSlot = null;

        infoPanel.SetLocked(false);
        foreach (CharacterSlotUI s in slots) s.SetLockState(false, false);

        // El foco vuelve al slot que estaba bloqueado
        EventSystem.current.SetSelectedGameObject(slot.gameObject);
    }

    private void Confirm(CharacterData data)
    {
        isLoading = true;
        GameSession.SelectedCharacter = data;
        SceneManager.LoadScene(gameSceneName);
    }
}