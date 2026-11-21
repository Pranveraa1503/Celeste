// ./Assets/Scripts/UI/MenuController.cs

using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuCanvas;

    private PlayerControls input;

    private void Awake()
    {
        input = new PlayerControls();

        if (menuCanvas == null)
        {
            Debug.LogError(
                $"[MenuController] Menu Canvas is not assigned on {gameObject.name}.",
                this
            );

            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (input == null)
            return;

        input.UI.ToggleMenu.performed += OnToggleMenu;
        input.UI.Enable();
    }

    private void OnDisable()
    {
        if (input == null)
            return;

        input.UI.ToggleMenu.performed -= OnToggleMenu;
        input.UI.Disable();
    }

    private void Start()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
    }

    private void OnToggleMenu(InputAction.CallbackContext context)
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
    }
}
