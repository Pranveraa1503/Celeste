// ./Assets/Data/ScriptableObjects/ItemDatabase.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Data/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items = new();

    // Cached lookup so FindItemById is O(1) instead of scanning the whole list.
    private Dictionary<string, ItemData> lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    /// <summary>
    /// Rebuilds the id -> ItemData cache and reports any problems found
    /// (missing ids, duplicate ids) via the console.
    /// </summary>
    private void BuildLookup()
    {
        lookup = new Dictionary<string, ItemData>();

        foreach (ItemData item in items)
        {
            if (item == null)
            {
                Debug.LogWarning($"[ItemDatabase] '{name}' contains a null item entry.", this);
                continue;
            }

            if (string.IsNullOrEmpty(item.id))
            {
                Debug.LogWarning($"[ItemDatabase] Item '{item.itemName}' has no id and will be skipped.", item);
                continue;
            }

            if (lookup.TryGetValue(item.id, out ItemData existing))
            {
                Debug.LogError(
                    $"[ItemDatabase] Duplicate item id '{item.id}' on '{item.itemName}' " +
                    $"- already used by '{existing.itemName}'. Fix one of the ids.",
                    item);
                continue;
            }

            lookup.Add(item.id, item);
        }
    }

    public ItemData FindItemById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        // Lazily rebuild if the cache was never created (e.g. asset created purely at runtime).
        if (lookup == null) BuildLookup();

        lookup.TryGetValue(id, out ItemData item);
        return item;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        BuildLookup();
    }
#endif
}
