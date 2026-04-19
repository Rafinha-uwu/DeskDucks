using TMPro;
using UnityEngine;

public class UpgradesPageUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private UpgradeCardUI cardPrefab;
    [SerializeField] private DuckUpgradeManager upgradeManager;

    [Header("Tooltip")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text tooltipText;

    void Start()
    {
        RefreshUI();
        HideTooltip();
    }

    public void RefreshUI()
    {
        Clear(contentRoot);

        if (upgradeManager == null)
            upgradeManager = DuckUpgradeManager.Instance;

        if (upgradeManager == null)
            return;

        var upgrades = upgradeManager.GetAllUpgrades();

        foreach (UpgradeData upgrade in upgrades)
        {
            if (upgrade == null)
                continue;

            UpgradeCardUI card = Instantiate(cardPrefab, contentRoot);
            card.Setup(upgrade, TryBuyUpgrade, ShowTooltip, HideTooltip);
        }
    }

    void TryBuyUpgrade(UpgradeData upgrade)
    {
        if (upgradeManager == null)
            upgradeManager = DuckUpgradeManager.Instance;

        if (upgradeManager == null)
            return;

        if (!upgradeManager.TryBuyUpgrade(upgrade))
            return;

        RefreshUI();
    }

    void ShowTooltip(string text)
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(true);

        if (tooltipText != null)
            tooltipText.text = text;
    }

    void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void Clear(Transform parent)
    {
        if (parent == null)
            return;

        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }
}