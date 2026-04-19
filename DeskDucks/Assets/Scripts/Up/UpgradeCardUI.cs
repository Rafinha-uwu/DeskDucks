using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UpgradeCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text costText;

    private UpgradeData data;
    private Action<UpgradeData> onClick;
    private Action<string> onHover;
    private Action onHoverExit;

    public void Setup(
        UpgradeData data,
        Action<UpgradeData> onClick,
        Action<string> onHover,
        Action onHoverExit)
    {
        this.data = data;
        this.onClick = onClick;
        this.onHover = onHover;
        this.onHoverExit = onHoverExit;

        if (icon != null)
            icon.sprite = data.icon;

        if (nameText != null)
            nameText.text = data.displayName;

        if (levelText != null)
            levelText.text = $"Level: {data.level}";

        if (costText != null)
            costText.text = $"{data.GetNextCost()}$";
    }

    public void OnClick()
    {
        onClick?.Invoke(data);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover?.Invoke(data.description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit?.Invoke();
    }
}