// ./Assets/Scripts/Systems/SaveController.cs
using UnityEngine;
using System;
using System.IO;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private InventoryController inventoryController;
    [SerializeField] private GameDatabase gameDatabase;

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        if (player == null)
            Debug.LogError($"[SaveController] player reference not assigned on {gameObject.name}.", this);
        if (inventoryController == null)
            Debug.LogError($"[SaveController] inventoryController reference not assigned on {gameObject.name}.", this);
        if (gameDatabase == null)
            Debug.LogError($"[SaveController] gameDatabase reference not assigned on {gameObject.name}.", this);
    }

    public void SaveGame()
    {
        if (player == null || inventoryController == null || inventoryController.Data == null)
        {
            Debug.LogError("[SaveController] Cannot save: missing player or inventory reference.");
            return;
        }

        try
        {
            SaveData saveData = new SaveData
            {
                playerPosition = player.position,
                // mapBoundary = FindObjectOfType<CinemachineVirtualCamera>().m_BoundingShape2D.gameObject.name;    // Cinemachine
                inventory = inventoryController.Data.ExportSaveData()
            };

            File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveController] Failed to save game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            SaveGame();
            return;
        }

        try
        {
            string json = File.ReadAllText(saveLocation);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
            {
                Debug.LogError("[SaveController] Save file parsed to null. Treating it as corrupted.");
                BackupCorruptedSave();
                return;
            }

            if (player != null)
            {
                player.position = saveData.playerPosition;
            }
            // FindObjectOfType<CinemachineConfiner>().m_BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();

            if (inventoryController != null && inventoryController.Data != null &&
                gameDatabase != null && gameDatabase.ItemDatabase != null)
            {
                inventoryController.Data.ImportSaveData(saveData.inventory, gameDatabase.ItemDatabase);
                inventoryController.RefreshUI();
            }
            else
            {
                Debug.LogWarning("[SaveController] Missing inventory/database references; inventory was not restored.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveController] Save file appears to be corrupted and could not be loaded: {e.Message}");
            BackupCorruptedSave();
        }
    }

    /// <summary>
    /// Moves an unreadable save file out of the way instead of leaving it in place
    /// to keep failing on every future load attempt.
    /// </summary>
    private void BackupCorruptedSave()
    {
        try
        {
            string backupPath = $"{saveLocation}.corrupted_{DateTime.Now:yyyyMMdd_HHmmss}";
            File.Move(saveLocation, backupPath);
            Debug.LogWarning($"[SaveController] Corrupted save moved to: {backupPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveController] Failed to back up corrupted save file: {e.Message}");
        }
    }
}
