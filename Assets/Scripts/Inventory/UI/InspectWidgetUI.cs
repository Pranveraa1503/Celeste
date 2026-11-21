using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InspectWidgetUI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float hiddenX = 1800f;
    [SerializeField] private float visibleX = 1350f;
    [SerializeField] private float animationDuration = 0.2f;

    [Header("UI References")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text infoText;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show(InventoryItem item)
    {
        if (item == null || item.IsEmpty)
            return;

        icon.sprite = item.data.icon;
        itemNameText.text = item.data.itemName;

        StringBuilder info = new StringBuilder();

        info.AppendLine(item.data.description);
        info.AppendLine();
        info.AppendLine($"Price: {item.data.price}");
        info.AppendLine($"Amount: {item.stackCount}");
        info.AppendLine($"Max Stack: {item.data.maxStack}");

        if (item.data.hasDurability)
        {
            info.AppendLine(
                $"Durability: {item.currentDurability} / {item.data.maxDurability}"
            );
        }

        infoText.text = info.ToString();

        rectTransform.DOAnchorPosX(
            visibleX,
            animationDuration
        );
    }

    public void Hide()
    {
        rectTransform.DOAnchorPosX(
            hiddenX,
            animationDuration
        );
    }
}