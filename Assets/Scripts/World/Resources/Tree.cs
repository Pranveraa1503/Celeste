// ./Assets/Scripts/World/Resources/Tree.cs
using UnityEngine;

public class Tree : MonoBehaviour, IInteractable
{
    [SerializeField]
    private TreeData data;
    public Vector2 Position => transform.position;

    private InventoryController inventory;

    private void Awake()
    {
        inventory = FindFirstObjectByType<InventoryController>();

        if (data == null)
            Debug.LogError($"[Tree] No TreeData assigned on {gameObject.name}.", this);
        if (inventory == null)
            Debug.LogError($"[Tree] No InventoryController found in the scene; loot cannot be granted.", this);
    }

    // NOTE: Loot is intentionally infinite for now (this Tree is being used as a
    // testing object while the inventory/loot systems are developed). Health/respawn
    // from TreeData are not wired up yet on purpose - see project notes.
    public void Interact()
    {
        if (data == null || inventory == null) return;

        foreach (var loot in data.lootTable)
        {
            if (loot == null || loot.item == null) continue;

            float chanceRoll = Random.Range(0f, 100f);
            if (chanceRoll > loot.chance)
            {
                continue;
            }

            int amount = Random.Range(loot.minAmount, loot.maxAmount + 1);
            inventory.AddItem(loot.item, amount);
            Debug.Log($"Rolled {chanceRoll} and got a {loot.item.itemName}\n with {amount} amount");
        }

        Debug.Log("Interacted with the tree!");
    }
}
