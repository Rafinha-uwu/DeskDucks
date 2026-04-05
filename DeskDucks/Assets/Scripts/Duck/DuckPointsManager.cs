using TMPro;
using UnityEngine;

public class DuckPointsManager : MonoBehaviour
{
    public static DuckPointsManager Instance { get; private set; }

    [Header("UI")]
    public TMP_Text pointsText;

    [Header("Points")]
    public int quackPoints = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateUI();
    }

    public void AddPoints(int amount)
    {
        quackPoints += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = $"QUACK Points: {quackPoints}";
    }
}