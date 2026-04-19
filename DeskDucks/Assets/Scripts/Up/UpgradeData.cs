using UnityEngine;

[System.Serializable]
public class UpgradeData
{
    public string id;
    public UpgradeType type;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Scaling")]
    public int baseCost = 10;
    public float costMultiplier = 1.5f;

    [Header("Value")]
    public int basePoints = 1;
    public int pointsPerLevel = 1;

    [HideInInspector] public int level = 1;

    public int GetCurrentPoints()
    {
        return basePoints + (level * pointsPerLevel);
    }

    public int GetNextCost()
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, level));
    }
}