// ./Assets/UI/HUD/HUDController.cs

/*
HUD Controller (UI Toolkit)

Responsibilities:
- Toggle the whole HUD panel (Tab key / close button)
- Switch between the 4 tabs (Player / Inventory / Map / Settings)
- Wire up the settings controls to basic, self-contained behaviour

This mirrors the same "one SelectTab() method" pattern used by the uGUI
TabController elsewhere in the project - just adapted to UI Toolkit's
VisualElement API instead of Image/GameObject toggling.
*/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class HUDController : MonoBehaviour
{
    private const string ActiveTabClass = "tab-button--active";
    private const string ActivePageClass = "page--active";
    private const string HiddenOverlayClass = "hud-overlay--hidden";

    private UIDocument document;
    private VisualElement root;
    private VisualElement overlay;

    private Button[] tabButtons;
    private VisualElement[] pages;
    private VisualElement tabIndicator;

    public int CurrentTab { get; private set; } = 0;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        if (document == null || document.rootVisualElement == null)
        {
            Debug.LogError($"[HUDController] UIDocument or its rootVisualElement is missing on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        root = document.rootVisualElement;
        overlay = root.Q<VisualElement>("hud-overlay");

        if (overlay == null)
        {
            Debug.LogError("[HUDController] Could not find 'hud-overlay' in the UXML hierarchy.", this);
            enabled = false;
            return;
        }

        tabButtons = new[]
        {
            root.Q<Button>("tab-player"),
            root.Q<Button>("tab-inventory"),
            root.Q<Button>("tab-map"),
            root.Q<Button>("tab-settings"),
        };

        pages = new[]
        {
            root.Q<VisualElement>("page-player"),
            root.Q<VisualElement>("page-inventory"),
            root.Q<VisualElement>("page-map"),
            root.Q<VisualElement>("page-settings"),
        };

        tabIndicator = root.Q<VisualElement>("tab-indicator");

        if (!ValidateSetup()) return;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i; // capture for the closure
            tabButtons[i].clicked += () => SelectTab(index);
        }

        Button closeButton = root.Q<Button>("close-button");
        if (closeButton != null) closeButton.clicked += HideHud;

        SetupSettingsPage();

        SelectTab(0);
        SetHudVisible(false); // HUD starts hidden; opened with Tab like the existing MenuController
    }

    private bool ValidateSetup()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] == null)
            {
                Debug.LogError($"[HUDController] Tab button at index {i} was not found in the UXML.", this);
                return false;
            }
        }

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] == null)
            {
                Debug.LogError($"[HUDController] Page at index {i} was not found in the UXML.", this);
                return false;
            }
        }

        if (tabButtons.Length != pages.Length)
        {
            Debug.LogError("[HUDController] Tab/page count mismatch.", this);
            return false;
        }

        return true;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SetHudVisible(overlay.ClassListContains(HiddenOverlayClass));
        }
    }

    // =========================
    // Tab switching
    // =========================

    public void SelectTab(int index)
    {
        if (index < 0 || index >= tabButtons.Length)
        {
            Debug.LogWarning($"[HUDController] Invalid tab index {index}.", this);
            return;
        }

        for (int i = 0; i < tabButtons.Length; i++)
        {
            bool isActive = i == index;
            tabButtons[i].EnableInClassList(ActiveTabClass, isActive);
            pages[i].EnableInClassList(ActivePageClass, isActive);
        }

        if (tabIndicator != null)
        {
            tabIndicator.style.left = new Length(index * 25f, LengthUnit.Percent);
        }

        CurrentTab = index;
    }

    // =========================
    // Panel visibility
    // =========================

    public void SetHudVisible(bool visible)
    {
        overlay.EnableInClassList(HiddenOverlayClass, !visible);
    }

    public void HideHud() => SetHudVisible(false);
    public void ShowHud() => SetHudVisible(true);

    // =========================
    // Settings page bindings
    // =========================

    private void SetupSettingsPage()
    {
        Slider masterSlider = root.Q<Slider>("master-volume-slider");
        Slider musicSlider = root.Q<Slider>("music-volume-slider");
        Slider sfxSlider = root.Q<Slider>("sfx-volume-slider");
        Toggle fullscreenToggle = root.Q<Toggle>("fullscreen-toggle");
        DropdownField languageDropdown = root.Q<DropdownField>("language-dropdown");
        Button applyButton = root.Q<Button>("settings-apply-button");
        Button backButton = root.Q<Button>("settings-back-button");

        if (languageDropdown != null)
        {
            languageDropdown.choices = new System.Collections.Generic.List<string> { "English", "العربية" };
            languageDropdown.index = 0;
        }

        // These hook straight into AudioListener volume as a sane default;
        // swap for your own AudioMixer exposed parameters when you add one.
        if (masterSlider != null)
            masterSlider.RegisterValueChangedCallback(evt => AudioListener.volume = evt.newValue);

        if (fullscreenToggle != null)
            fullscreenToggle.RegisterValueChangedCallback(evt => Screen.fullScreen = evt.newValue);

        if (applyButton != null)
            applyButton.clicked += () => Debug.Log("[HUDController] Settings applied (wire this up to your save/settings system).");

        if (backButton != null)
            backButton.clicked += () => SelectTab(0);

        // musicSlider / sfxSlider are left unwired since there's no audio mixer
        // reference yet - grab them here once one exists.
        _ = musicSlider;
        _ = sfxSlider;
    }
}