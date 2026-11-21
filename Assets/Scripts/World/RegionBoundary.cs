// ./Assets/Scripts/World/RegionBoundary.cs
using UnityEngine;

public enum BoundaryAxis
{
    Horizontal, // عرضي: بيمتد عرضياً على الشاشة (محور Y)
    Vertical    // طولي: بيمتد طولياً على الشاشة (محور X)
}

public class RegionBoundary : MonoBehaviour
{
    [Header("Connected Regions")]
    [Tooltip("Convention: for Horizontal axis, Region A must be the one above and Region B below. " +
             "For Vertical axis, Region A must be to the left and Region B to the right.")]
    [SerializeField] private Region regionA;
    [SerializeField] private Region regionB;

    [Header("Transition Settings")]
    [SerializeField] private BoundaryAxis axis;
    [SerializeField] private float transitionOffset = 0.3f;

    // الـ Cooldown Flag
    private bool canTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. لو مقفول، م تعملش حاجة
        if (!canTrigger) return;

        if (!collision.TryGetComponent<PlayerMovement>(out _))
            return;

        if (regionA == null || regionB == null)
        {
            Debug.LogError("[RegionBoundary] One or more Regions are not assigned.", this);
            return;
        }

        if (WorldManager.Instance == null)
        {
            Debug.LogError("[RegionBoundary] WorldManager not found.", this);
            return;
        }

        Region current = WorldManager.Instance.CurrentRegion;

        if (current == null)
        {
            Debug.LogError("[RegionBoundary] Current Region is null.", this);
            return;
        }

        // تحديد المنطقة المستهدفة
        Region targetRegion = null;

        if (current == regionA) targetRegion = regionB;
        else if (current == regionB) targetRegion = regionA;
        else
        {
            Debug.LogError(
                $"Player is in '{current.DisplayName}' but entered boundary between '{regionA.DisplayName}' and '{regionB.DisplayName}'.",
                this);
            return;
        }

        // 2. اقفل الـ Trigger عشان م يشتغلش تاني
        canTrigger = false;

        // تغيير المنطقة
        WorldManager.Instance.ChangeRegion(targetRegion);

        // دفع اللاعب قليلاً عشان يفوق من الـ Trigger
        Rigidbody2D rb = collision.attachedRigidbody;

        if (rb != null)
        {
            Vector2 pushDirection;

            if (axis == BoundaryAxis.Horizontal)
            {
                // A أعلى - B أسفل
                pushDirection = (current == regionA)
                    ? Vector2.down
                    : Vector2.up;
            }
            else
            {
                // A يسار - B يمين
                pushDirection = (current == regionA)
                    ? Vector2.right
                    : Vector2.left;
            }

            rb.MovePosition(
                rb.position + pushDirection * transitionOffset
            );
        }

        Debug.Log($"Player entered region '{targetRegion.DisplayName}'.");
    }

    // 3. لما اللاعب يخرج من الـ Trigger، افتحه تاني
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovement>(out _))
        {
            canTrigger = true;
        }
    }
}