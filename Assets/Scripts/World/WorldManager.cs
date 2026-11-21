// ./Assets/Scripts/World/WorldManager.cs

using System;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    [Header("State")]
    [SerializeField] private Region currentRegion;

    public event Action<Region> OnRegionChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (currentRegion != null)
        {
            OnRegionChanged?.Invoke(currentRegion);
        }
    }

    public void ChangeRegion(Region newRegion)
    {
        if (newRegion == null)
        {
            Debug.LogError("[WorldManager] Cannot change to a null Region.");
            return;
        }

        if (newRegion == currentRegion)
        {
            // Debug.LogWarning("[WorldManager] Attempted to change to the current Region.");
            return;
        }

        currentRegion = newRegion;

        Debug.Log($"[WorldManager] Entered Region: {currentRegion.DisplayName}");

        OnRegionChanged?.Invoke(currentRegion);
    }

    public Region CurrentRegion => currentRegion;
}