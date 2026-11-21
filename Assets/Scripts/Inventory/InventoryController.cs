// ./Assets/Scripts/Inventory/InventoryController.cs

/*
Inventory Controller
- Add Item
- Remove Item
- Move Item
- Swap
- Inventory Data

Don't know Image or TMP_Text.
*/
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Settings")]
    [SerializeField] private int slotCount = 40;

    // الـ Model (الداتا الحقيقية)
    public InventoryData Data { get; private set; }

    private void Start()
    {
        if (inventoryUI == null)
        {
            Debug.LogError($"[InventoryController] inventoryUI is not assigned on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        // أي وقت الـ Inventory UI بيظهر (فتح الصفحة)، نعيد دفع الداتا للشاشة
        // عشان الـ UI يفضل دايماً مطابق لـ InventoryData مهما كانت حالة الـ Load.
        inventoryUI.UIVisible += RefreshUI;

        // 1. بناء الداتا
        Data = new InventoryData(slotCount);

        // 2. بناء الـ UI وباصيله نفسه عشان الـ Slots توصل له من غير FindObject
        inventoryUI.CreateSlots(slotCount, this);

        // 3. عمل Refresh عشان يطابق الداتا (في البداية هتكون فاضية)
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (inventoryUI != null)
            inventoryUI.UIVisible -= RefreshUI;
    }

    // إضافة آيتم (للاستخدام من السكريبتات التانية زي PlayerInteraction)
    public void AddItem(ItemData item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("[InventoryController] AddItem called with a null item.");
            return;
        }

        int remaining = Data.AddItem(item, amount);
        if (remaining > 0)
        {
            Debug.Log($"Inventory Full! Couldn't add {remaining} {item.itemName}s.");
        }
        RefreshUI();
    }

    // مسح آيتم
    public void RemoveItem(int slotIndex, int amount = 1)
    {
        Data.RemoveItem(slotIndex, amount);
        RefreshUI();
    }

    // تحديث الشاشة بناءً على الداتا
    public void RefreshUI()
    {

        Debug.Log(
            $"[InventoryController] RefreshUI | " +
            $"Data: {Data != null} | " +
            $"UI: {inventoryUI != null} | " +
            $"Controller Active: {gameObject.activeInHierarchy} | " +
            $"UI Active: {(inventoryUI != null && inventoryUI.gameObject.activeInHierarchy)}",
            this
        );
        if (Data == null || inventoryUI == null) return;

        for (int i = 0; i < Data.SlotCount; i++)
        {
            InventoryItem item = Data.GetItem(i);
            InventorySlotUI slotUI = inventoryUI.GetSlot(i);

            if (slotUI == null)
            {
                Debug.LogError($"Slot {i} is NULL!");
                continue;
            }

            if (item != null && !item.IsEmpty)
            {
                Debug.Log(
                    $"[InventoryController] Refresh slot {i} | " +
                    $"Item: {item.data.itemName} | " +
                    $"Slot Active: {slotUI.gameObject.activeInHierarchy}",
                    slotUI
                );

                slotUI.SetSlot(item.data.icon, item.stackCount);
            }
            else
            {
                slotUI.ClearSlot();
            }
        }
    }

    // ترتيب الآيتمات (للاستخدام من الـ Drag Handler)
    public void SwapItems(int sourceIndex, int targetIndex)
    {
        Data.SwapItems(sourceIndex, targetIndex);
        RefreshUI();
    }
}


/*
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Settings")]
    [SerializeField] private int slotCount = 40;

    public InventoryData Data { get; private set; }

    private void Start()
    {
        if (inventoryUI == null)
        {
            Debug.LogError($"[InventoryController] inventoryUI is not assigned on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        Data = new InventoryData(slotCount);
        inventoryUI.CreateSlots(slotCount, this);
        RefreshUI();
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (item == null) return;
        int remaining = Data.AddItem(item, amount);
        if (remaining > 0) Debug.Log($"Inventory Full! Couldn't add {remaining} {item.itemName}s.");
        RefreshUI();
    }

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        Data.RemoveItem(slotIndex, amount);
        RefreshUI();
    }

    // ======= الجديد: استخدام الآيتم (Use Item) =======
    public void UseItem(int slotIndex, GameObject playerTarget)
    {
        InventoryItem item = Data.GetItem(slotIndex);
        if (item == null || item.IsEmpty) return;

        if (item.data.category == ItemCategory.Consumable)
        {
            // Apply Effects (زي الـ JS switch case)
            if (item.data.effects != null)
            {
                foreach (var effect in item.data.effects)
                {
                    switch (effect.type)
                    {
                        case EffectType.InstantHeal:
                            // playerTarget.GetComponent<PlayerHealth>()?.Heal(effect.amount);
                            Debug.Log($"Healed {effect.amount} HP!");
                            break;

                        case EffectType.RestoreMana:
                            // playerTarget.GetComponent<PlayerMana>()?.Restore(effect.amount);
                            Debug.Log($"Restored {effect.amount} Mana!");
                            break;
                    }
                }
            }

            // بعد ما استخدمناه، نمسحه من الـ Inventory
            RemoveItem(slotIndex);
        }
        else if (item.data.category == ItemCategory.Equipment)
        {
            Debug.Log("Equipping items is not implemented yet!");
            // playerTarget.GetComponent<EquipmentSystem>()?.Equip(item);
        }
    }

    public void RefreshUI()
    {
        if (Data == null || inventoryUI == null) return;

        for (int i = 0; i < Data.SlotCount; i++)
        {
            InventoryItem item = Data.GetItem(i);
            InventorySlotUI slotUI = inventoryUI.GetSlot(i);

            if (slotUI == null) continue;

            if (item != null && !item.IsEmpty)
            {
                slotUI.SetSlot(item.data.icon, item.stackCount);
            }
            else
            {
                slotUI.ClearSlot();
            }
        }
    }

    public void SwapItems(int sourceIndex, int targetIndex)
    {
        Data.SwapItems(sourceIndex, targetIndex);
        RefreshUI();
    }
}
*/
