// ./Assets/Scripts/Inventory/UI/InventoryScrollSnap.cs

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class InventoryScrollSnap : MonoBehaviour, IScrollHandler
{
    [Header("References")]
    [SerializeField] private GridLayoutGroup grid;

    private ScrollRect scrollRect;

    private int currentRow;
    private int totalRows;
    private int visibleRows;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        if (grid == null)
        {
            Debug.LogError(
                "[InventoryScrollSnap] GridLayoutGroup is not assigned.",
                this);
        }
    }

    private void Start()
    {
        RefreshLayout();
        SnapToRow(0);
    }

    public void RefreshLayout()
    {
        if (grid == null || scrollRect.viewport == null)
            return;

        RectTransform content = grid.GetComponent<RectTransform>();

        if (content == null)
            return;

        float rowHeight = grid.cellSize.y + grid.spacing.y;

        if (rowHeight <= 0f)
            return;

        totalRows = Mathf.Max(
            1,
            Mathf.CeilToInt(
                (content.rect.height - grid.padding.top - grid.padding.bottom + grid.spacing.y)
                / rowHeight
            )
        );

        visibleRows = Mathf.Max(
            1,
            Mathf.FloorToInt(
                (scrollRect.viewport.rect.height + grid.spacing.y)
                / rowHeight
            )
        );

        int maxStartRow = Mathf.Max(
            0,
            totalRows - visibleRows
        );

        currentRow = Mathf.Clamp(
            currentRow,
            0,
            maxStartRow
        );
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (eventData.scrollDelta.y == 0f)
            return;

        RefreshLayout();

        if (eventData.scrollDelta.y < 0f)
        {
            ScrollDown();
        }
        else
        {
            ScrollUp();
        }
    }

    private void ScrollDown()
    {
        int maxStartRow = Mathf.Max(
            0,
            totalRows - visibleRows
        );

        if (currentRow >= maxStartRow)
            return;

        currentRow++;

        SnapToRow(currentRow);
    }

    private void ScrollUp()
    {
        if (currentRow <= 0)
            return;

        currentRow--;

        SnapToRow(currentRow);
    }

    private void SnapToRow(int row)
    {
        int maxStartRow = Mathf.Max(
            0,
            totalRows - visibleRows
        );

        currentRow = Mathf.Clamp(
            row,
            0,
            maxStartRow
        );

        float normalizedPosition;

        if (maxStartRow == 0)
        {
            normalizedPosition = 1f;
        }
        else
        {
            normalizedPosition =
                1f - ((float)currentRow / maxStartRow);
        }

        scrollRect.StopMovement();
        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }
}