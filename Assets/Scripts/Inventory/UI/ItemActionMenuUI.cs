// ./Assets/Scripts/UI/ItemActionMenuUI.cs
using DG.Tweening;
using UnityEngine;

public class ItemActionMenuUI : MonoBehaviour
{
    [SerializeField] private float hiddenY = -200f;
    [SerializeField] private float visibleY = 50f;
    [SerializeField] private float animationDuration = 0.2f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show()
    {
        rectTransform.DOAnchorPosY(
            visibleY,
            animationDuration
        );
    }
}