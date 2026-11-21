using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot Settings")]
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField] private Transform gridParent;

    [Header("UI References")]
    [SerializeField] private ItemActionMenuUI actionMenu;
    [SerializeField] private InspectWidgetUI inspectWidget;
    [SerializeField] private GridLayoutGroup grid;

    private const int NormalColumnCount = 10;
    private const int InspectColumnCount = 8;

    private readonly List<InventorySlotUI> slots = new();

    private InventoryController controller;
    private InventorySlotUI selectedSlot;

    public event System.Action UIVisible;

    private void OnEnable()
    {
        UIVisible?.Invoke();
    }

    public void CreateSlots(int count, InventoryController controller)
    {
        if (slotPrefab == null || gridParent == null)
        {
            Debug.LogError(
                $"[InventoryUI] slotPrefab or gridParent not assigned on {gameObject.name}.",
                this);
            return;
        }

        this.controller = controller;

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        slots.Clear();
        selectedSlot = null;

        for (int i = 0; i < count; i++)
        {
            InventorySlotUI slot = Instantiate(slotPrefab, gridParent);
            slot.name = $"Slot ({i})";

            slot.Initialize(i, controller);

            slots.Add(slot);
        }
    }

    public InventorySlotUI GetSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return null;

        return slots[index];
    }

    public void SelectSlot(InventorySlotUI slot)
    {
        if (slot == null)
            return;

        if (selectedSlot == slot)
            return;

        if (selectedSlot != null)
            selectedSlot.SetSelected(false);

        selectedSlot = slot;
        selectedSlot.SetSelected(true);

        if (actionMenu == null)
        {
            Debug.LogError(
                $"[InventoryUI] Action Menu is not assigned on {gameObject.name}.",
                this);
            return;
        }

        actionMenu.Show();
    }

    public void SetInspectLayout(bool inspecting)
    {
        if (grid == null)
            return;

        grid.constraintCount =
            inspecting
                ? InspectColumnCount
                : NormalColumnCount;
    }

    public void OpenInspect(int slotIndex)
    {
        if (controller == null)
        {
            Debug.LogError(
                $"[InventoryUI] InventoryController is not assigned on {gameObject.name}.",
                this);
            return;
        }

        if (inspectWidget == null)
        {
            Debug.LogError(
                $"[InventoryUI] Inspect Widget is not assigned on {gameObject.name}.",
                this);
            return;
        }

        InventoryItem item = controller.Data.GetItem(slotIndex);

        if (item == null || item.IsEmpty)
            return;

        InventorySlotUI slot = GetSlot(slotIndex);

        if (slot != null)
            SelectSlot(slot);

        SetInspectLayout(true);
        inspectWidget.Show(item);
    }
}