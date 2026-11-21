// ./Assets/Data/ScriptableObjects/ItemData.cs

/*
Item Data
- Name
- Description
- Price
- Icon
- Max Stack
- Durability

he know itself.
*/

using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique identifier used for save data and database lookups. " +
             "Leave empty to auto-generate one from the item name.")]
    public string id;
    public string itemName;
    [TextArea]
    public string description;

    [Header("Economy")]
    public int price;
    public Sprite icon;

    [Header("Stacking")]
    [Min(1)] public int maxStack = 99;
    public bool isStackable => maxStack > 1;

    [Header("Durability")]
    [Tooltip("Enable if this item degrades with use (tools, weapons, armor).")]
    public bool hasDurability = false;
    [Min(1)] public int maxDurability = 100;

    /// <summary>
    /// Deterministically generates a safe id from the given display name:
    /// lowercase, spaces -> underscores, invalid characters stripped.
    /// </summary>
    public static string GenerateIdFromName(string source)
    {
        if (string.IsNullOrWhiteSpace(source)) return string.Empty;

        string result = source.Trim().ToLowerInvariant();
        result = Regex.Replace(result, @"\s+", "_");
        result = Regex.Replace(result, @"[^a-z0-9_]", "");
        result = Regex.Replace(result, @"_+", "_").Trim('_');

        return result;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Only auto-generate if the id is empty - never overwrite a manually set id.
        if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(itemName))
        {
            id = GenerateIdFromName(itemName);
        }

        if (maxStack < 1) maxStack = 1;
        if (maxDurability < 1) maxDurability = 1;
        if (price < 0) price = 0;
    }
#endif
}


/*
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public enum ItemCategory { Material, Consumable, Equipment }
public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }
public enum EquipmentSlot { None, Weapon, Head, Chest, Legs, Accessory }
public enum EffectType { InstantHeal, RestoreMana }

[Serializable]
public class ItemEffect
{
    public EffectType type;
    public int amount;
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string id;
    public string itemName;
    [TextArea] public string description;
    
    [Header("Classification")]
    public ItemCategory category;
    public Rarity rarity = Rarity.Common;

    [Header("Economy")]
    public int price;
    public Sprite icon;

    [Header("Stacking")]
    [Min(1)] public int maxStack = 99;
    public bool isStackable => maxStack > 1;

    [Header("Durability")]
    public bool hasDurability = false;
    [Min(1)] public int maxDurability = 100;

    // ======= COMPOSITION BLOCKS (بدل الـ Inheritance) =======

    [Header("Consumable Effects (لو الآيتم Consumable)")]
    public ItemEffect[] effects; // زي الـ JS بتاعتك

    [Header("Equipment Data (لو الآيتم Equipment)")]
    public EquipmentSlot slot = EquipmentSlot.None;
    public int attackBonus = 0;
    public int defenseBonus = 0;
    // ممكن تضيف statsBonus Dictionary أو Struct زي الـ JS

    // ======= Auto ID Generation =======
    public static string GenerateIdFromName(string source)
    {
        if (string.IsNullOrWhiteSpace(source)) return string.Empty;
        string result = source.Trim().ToLowerInvariant();
        result = Regex.Replace(result, @"\s+", "_");
        result = Regex.Replace(result, @"[^a-z0-9_]", "");
        result = Regex.Replace(result, @"_+", "_").Trim('_');
        return result;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(itemName))
            id = GenerateIdFromName(itemName);

        if (maxStack < 1) maxStack = 1;
        if (maxDurability < 1) maxDurability = 1;
        if (price < 0) price = 0;

        // لو الآيتم Equipment، خليه ميتعملش Stack
        if (category == ItemCategory.Equipment) maxStack = 1;
    }
#endif
}
*/