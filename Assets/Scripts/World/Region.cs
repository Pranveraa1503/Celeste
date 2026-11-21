// ./Assets/Scripts/World/Region.cs
using UnityEngine;

public class Region : MonoBehaviour
{
    [Header("Region Data")]
    [Tooltip("Unique identifier used by save/load and future game systems.")]
    [SerializeField] private string regionId;
    [SerializeField] private string displayName;
    
    [Tooltip("Auto-filled from children. You can also assign it manually if needed.")]
    [SerializeField] private Collider2D cameraBounds;

    // Read-Only Access
    public string RegionId => regionId;
    public string DisplayName => displayName;
    public Collider2D CameraBounds => cameraBounds;

#if UNITY_EDITOR
    // بتتنفذ أول ما تضيف السكريبت على GameObject في الـ Inspector
    private void Reset()
    {
        AutoFetchBounds();
    }

    // بتتنفذ كل ما تغير أي حاجة في الـ Inspector (أو لما تضغط Save)
    private void OnValidate()
    {
        // لو فارغة، دور عليها لوحدك
        if (cameraBounds == null)
        {
            AutoFetchBounds();
        }
    }

    private void AutoFetchBounds()
    {
        // بنبحث عن أي Collider2D في الأبناء
        // cameraBounds = GetComponentInChildren<Collider2D>();
        cameraBounds = GetComponentInChildren<Collider2D>(true);

        if (cameraBounds == null)
        {
            Debug.LogWarning($"[Region] No Collider2D found in children of '{gameObject.name}' to use as CameraBounds.");
        }
    }
#endif

    // Draws the actual CameraBounds shape in the Scene view so you can visually confirm
    // it covers the right area and isn't a leftover default-sized collider.
    private void OnDrawGizmos()
    {
        if (cameraBounds == null) return;

        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.9f);
        Bounds b = cameraBounds.bounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }
}