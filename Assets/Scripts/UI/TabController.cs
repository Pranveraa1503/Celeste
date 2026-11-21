// ./Assets/Scripts/UI/TabController.cs
// Controls tab switching in the menu.
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [SerializeField] private Image[] tabImages;
    [SerializeField] private GameObject[] pages;

    [Header("Visuals")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;

    public int CurrentTab { get; private set; } = -1;

    private void Start()
    {
        SelectTab(0);
    }

    /// <summary>
    /// Single entry point for tab switching - wire this to your tab buttons'
    /// OnClick events (instead of the old SwitchTab/ActivateTab).
    /// </summary>
    public void SelectTab(int tabIndex)
    {
        if (!IsSetupValid()) return;

        if (tabIndex < 0 || tabIndex >= pages.Length)
        {
            Debug.LogWarning($"[TabController] Invalid tab index {tabIndex} on {gameObject.name}.", this);
            return;
        }

        for (int i = 0; i < pages.Length; i++)
        {
            bool isActive = i == tabIndex;

            if (pages[i] != null)
                pages[i].SetActive(isActive);

            if (tabImages[i] != null)
                tabImages[i].color = isActive ? activeColor : inactiveColor;
        }

        CurrentTab = tabIndex;
    }

    private bool IsSetupValid()
    {
        if (tabImages == null || pages == null || tabImages.Length == 0 || pages.Length == 0)
        {
            Debug.LogError($"[TabController] Tab images or pages are not assigned on {gameObject.name}.", this);
            return false;
        }

        if (tabImages.Length != pages.Length)
        {
            Debug.LogError(
                $"[TabController] tabImages ({tabImages.Length}) and pages ({pages.Length}) " +
                $"length mismatch on {gameObject.name}.", this);
            return false;
        }

        return true;
    }
}
