// Assets/Scripts/Systems/GameDatabase.cs
using UnityEngine;

public class GameDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemDatabase itemDatabase;

    public ItemDatabase ItemDatabase => itemDatabase;

    private void Awake()
    {
        if (itemDatabase == null)
        {
            Debug.LogError($"[GameDatabase] itemDatabase is not assigned on {gameObject.name}.", this);
        }
    }
}
