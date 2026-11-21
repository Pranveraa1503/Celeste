// ./Assets/Scripts/Inventory/InventoryItem.cs
/*
Inventory Item
- Icon Image
- Count Text
- ItemData
- Stack Count
- Durability (when the item supports it)

he know itself.
*/

using UnityEngine;

// ده Instance من الآيتم اللي موجود في الـ Inventory
public class InventoryItem
{
    public ItemData data; // معلومات الآيتم الأساسية
    public int stackCount; // العدد الحالي
    public int currentDurability; // يُستخدم فقط لو data.hasDurability

    // Constructor
    public InventoryItem(ItemData itemData, int amount = 1)
    {
        data = itemData;
        stackCount = amount;
        currentDurability = (data != null && data.hasDurability) ? data.maxDurability : 0;
    }

    // هل الخانة فاضية؟
    public bool IsEmpty => data == null;

    // إضافة عدد للـ Stack
    public void AddToStack(int amount)
    {
        if (data == null) return;

        stackCount += amount;
        if (stackCount > data.maxStack) stackCount = data.maxStack;
    }

    /// <summary>
    /// Reduces durability, clamped to zero. No-op for items without durability.
    /// </summary>
    public void ReduceDurability(int amount)
    {
        if (data == null || !data.hasDurability) return;

        currentDurability = Mathf.Max(0, currentDurability - amount);
    }

    public bool IsBroken => data != null && data.hasDurability && currentDurability <= 0;
}
