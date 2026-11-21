using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    // =========================
    // References
    // =========================

    private PlayerControls input;
    private PlayerMovement movement;

    [SerializeField]
    private GameObject interactUI;

    // =========================
    // State
    // =========================

    private IInteractable currentTarget;
    private List<IInteractable> interactablesInRange = new();

    // =========================
    // Unity Lifecycle
    // =========================

    private void Awake()
    {
        // Create Input
        input = new PlayerControls();
        // Get PlayerMovement
        movement = GetComponent<PlayerMovement>();

        // Hide UI
        if (interactUI != null)
        {
            interactUI.SetActive(false);
        }
        else
        {
            Debug.LogError($"[PlayerInteraction] interactUI is not assigned on {gameObject.name}.", this);
        }
    }

    private void OnEnable()
    {
        // Enable Input
        input.Gameplay.Enable();
    }

    private void OnDisable()
    {
        // Disable Input
        input.Gameplay.Disable();
    }

    private void Update()
    {
        // Interactables can be destroyed/disabled without ever firing OnTriggerExit2D
        // (e.g. Destroy() while overlapping), so we sweep for stale entries every frame
        // before deciding whether to act.
        PruneInvalidTargets();

        bool interactTriggered = input.Gameplay.Interact.triggered;

        if (interactTriggered && IsTargetValid(currentTarget))
        {
            currentTarget.Interact();
        }
    }

    // =========================
    // Trigger Detection
    // =========================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null && !interactablesInRange.Contains(interactable))
        {
            interactablesInRange.Add(interactable);
            ChooseBestTarget();
            UpdateInteractionUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;

        IInteractable interactable = other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactablesInRange.Remove(interactable);
            ChooseBestTarget();
            UpdateInteractionUI();
        }
    }

    // =========================
    // Internal Logic
    // =========================

    /// <summary>
    /// True when the interface reference points to a live, non-destroyed object.
    /// Interfaces don't go through UnityEngine.Object's overloaded null check,
    /// so a destroyed MonoBehaviour can look "not null" through its interface
    /// reference unless we explicitly check it as a UnityEngine.Object.
    /// </summary>
    private bool IsTargetValid(IInteractable target)
    {
        if (target == null) return false;
        if (target is Object unityObject && unityObject == null) return false;
        return true;
    }

    /// <summary>
    /// Removes any destroyed/disabled interactables that never triggered
    /// OnTriggerExit2D, and refreshes the current target/UI if anything changed.
    /// </summary>
    private void PruneInvalidTargets()
    {
        int removed = interactablesInRange.RemoveAll(t => !IsTargetValid(t));

        if (removed > 0 || !IsTargetValid(currentTarget))
        {
            ChooseBestTarget();
            UpdateInteractionUI();
        }
    }

    private void ChooseBestTarget()
    {
        // لا يوجد أي عنصر داخل الرينج
        if (interactablesInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }

        IInteractable bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (IInteractable interactable in interactablesInRange)
        {
            if (!IsTargetValid(interactable)) continue;

            float distance = Vector2.Distance(
                transform.position,
                interactable.Position
            );

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = interactable;
            }
        }

        currentTarget = bestTarget;
    }

    private void UpdateInteractionUI()
    {
        if (interactUI == null) return;

        if (!IsTargetValid(currentTarget))
        {
            interactUI.SetActive(false);
            return;
        }

        interactUI.SetActive(true);

        if (Camera.main == null) return;

        Vector3 screenPosition =
            Camera.main.WorldToScreenPoint(
                currentTarget.Position + Vector2.up * 1.2f
            );

        interactUI.transform.position = screenPosition;
    }
}
