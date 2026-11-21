// ./Assets/Scripts/UI/InventorySlotUI.cs

/*
Inventory Slot UI
- Selection
- Highlight
- Hover

he don't know Image(Icon) or TMP_Text(count).
*/

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Image selection;

    public int SlotIndex { get; private set; }

    public InventoryController Controller { get; private set; }

    public bool HasItem { get; private set; }

    public void Initialize(int index, InventoryController controller)
    {
        SlotIndex = index;
        Controller = controller;
    }

    public void SetSlot(Sprite itemIcon, int amount)
    {
        if (icon == null)
        {
            Debug.LogError(
                $"[InventorySlotUI] Icon reference missing on {gameObject.name}.",
                this);
            return;
        }

        if (itemIcon == null)
        {
            ClearSlot();
            return;
        }

        HasItem = true;

        icon.sprite = itemIcon;
        icon.enabled = true;

        if (countText != null)
        {
            if (amount > 1)
            {
                countText.text = amount.ToString();
                countText.enabled = true;
            }
            else
            {
                countText.text = "";
                countText.enabled = false;
            }
        }
    }

    public void ClearSlot()
    {
        HasItem = false;

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (countText != null)
        {
            countText.text = "";
            countText.enabled = false;
        }

        SetSelected(false);
    }

    public void SetSelected(bool value)
    {
        if (selection != null)
            selection.enabled = value;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!HasItem)
            return;

        InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();

        if (inventoryUI == null)
        {
            Debug.LogError(
                $"[InventorySlotUI] Could not find InventoryUI for {gameObject.name}.",
                this);
            return;
        }

        if (eventData.clickCount >= 2)
        {
            inventoryUI.OpenInspect(SlotIndex);
            return;
        }

        inventoryUI.SelectSlot(this);
    }
}
