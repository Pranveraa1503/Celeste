// ./Assets/Scripts/Camera/CameraRegionController.cs

using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineConfiner2D))]
[RequireComponent(typeof(CinemachineCamera))]
public class CameraRegionController : MonoBehaviour
{
    private CinemachineConfiner2D confiner;
    private CinemachineCamera virtualCamera;

    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
        virtualCamera = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        if (WorldManager.Instance == null)
        {
            Debug.LogError("[CameraRegionController] WorldManager not found.", this);
            enabled = false;
            return;
        }

        // Subscribed here (not OnEnable) so we're guaranteed WorldManager.Instance is
        // already set - Unity runs every Awake() before any Start() in the scene.
        WorldManager.Instance.OnRegionChanged += HandleRegionChanged;

        if (WorldManager.Instance.CurrentRegion != null)
        {
            ApplyBounds(WorldManager.Instance.CurrentRegion);
        }
    }

    private void OnDisable()
    {
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnRegionChanged -= HandleRegionChanged;
        }
    }

    private void HandleRegionChanged(Region region)
    {
        if (region == null || region.CameraBounds == null)
        {
            Debug.LogWarning("[CameraRegionController] Region or CameraBounds is missing.", this);
            return;
        }

        // Swap the confiner shape immediately - this is the pattern Cinemachine's own
        // room/region-swap samples use. No manual unconfine/wait step needed: CameraBounds
        // is inset from the walkable area on purpose (it constrains the camera's center,
        // not the player), so waiting for the player to "enter" it can time out even when
        // everything is set up correctly. The Position Composer's own damping already
        // smooths the visual re-clamp into the new shape as the player keeps moving.
        ApplyBounds(region);
    }

    private void ApplyBounds(Region region)
    {
        confiner.BoundingShape2D = region.CameraBounds;
        confiner.InvalidateBoundingShapeCache();

        // Prevents Cinemachine from interpolating from the old (now invalid) confined
        // state, which could otherwise cause the camera to slide along the wrong shape
        // for a frame or two right after the swap.
        virtualCamera.PreviousStateIsValid = false;
    }
}