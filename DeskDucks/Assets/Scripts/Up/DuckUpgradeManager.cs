using System.Collections.Generic;
using UnityEngine;

public class DuckUpgradeManager : MonoBehaviour
{
    public static DuckUpgradeManager Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private List<UpgradeData> upgrades = new();

    private readonly Dictionary<UpgradeType, UpgradeData> upgradeLookup = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        BuildLookup();
    }

    void BuildLookup()
    {
        upgradeLookup.Clear();

        foreach (UpgradeData upgrade in upgrades)
        {
            if (upgrade == null)
                continue;

            if (!upgradeLookup.ContainsKey(upgrade.type))
                upgradeLookup.Add(upgrade.type, upgrade);
        }
    }

    public List<UpgradeData> GetAllUpgrades()
    {
        return upgrades;
    }

    public UpgradeData GetUpgrade(UpgradeType type)
    {
        upgradeLookup.TryGetValue(type, out UpgradeData upgrade);
        return upgrade;
    }

    public int GetCurrentPointsFor(UpgradeType type)
    {
        UpgradeData upgrade = GetUpgrade(type);
        if (upgrade == null)
            return 1;

        return upgrade.GetCurrentPoints();
    }

    public bool TryBuyUpgrade(UpgradeData upgrade)
    {
        if (upgrade == null)
            return false;

        if (DuckPointsManager.Instance == null)
            return false;

        int cost = upgrade.GetNextCost();

        if (!DuckPointsManager.Instance.SpendPoints(cost))
            return false;

        upgrade.level++;
        return true;
    }
}