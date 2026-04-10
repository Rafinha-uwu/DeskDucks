using TMPro;
using UnityEngine;

public class DuckPointsManager : MonoBehaviour
{
    public static DuckPointsManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text pointsText;

    [Header("Points")]
    [SerializeField] private int quackPoints = 0;

    public int QuackPoints => quackPoints;

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

    public bool SpendPoints(int amount)
    {
        if (amount <= 0)
            return true;

        if (quackPoints < amount)
            return false;

        quackPoints -= amount;
        UpdateUI();
        return true;
    }

    public void SetPoints(int amount)
    {
        quackPoints = Mathf.Max(0, amount);
        UpdateUI();
    }

    public void ResetPoints()
    {
        quackPoints = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = $"QUACK Points: {quackPoints}";
    }
}