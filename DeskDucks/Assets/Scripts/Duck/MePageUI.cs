using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MePageUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField duckNameInput;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Slider levelProgressBar;
    [SerializeField] private TMP_Text timePlayedText;
    [SerializeField] private TMP_Text lifetimePointsText;

    private DuckProfileManager profileManager;

    void Start()
    {
        profileManager = DuckProfileManager.Instance;

        if (profileManager != null && duckNameInput != null)
        {
            duckNameInput.SetTextWithoutNotify(profileManager.DuckName);
            duckNameInput.onEndEdit.AddListener(HandleNameChanged);
        }

        RefreshUI();
    }

    void OnEnable()
    {
        RefreshUI();
    }

    void OnDestroy()
    {
        if (duckNameInput != null)
            duckNameInput.onEndEdit.RemoveListener(HandleNameChanged);
    }

    void Update()
    {
        RefreshUI();
    }

    void HandleNameChanged(string newName)
    {
        if (profileManager == null)
            profileManager = DuckProfileManager.Instance;

        if (profileManager == null)
            return;

        profileManager.SetDuckName(newName);

        if (duckNameInput != null)
            duckNameInput.SetTextWithoutNotify(profileManager.DuckName);
    }

    void RefreshUI()
    {
        if (profileManager == null)
            profileManager = DuckProfileManager.Instance;

        if (profileManager == null)
            return;

        if (levelText != null)
            levelText.text = $"Level {profileManager.CurrentLevel}";

        if (levelProgressBar != null)
            levelProgressBar.value = profileManager.GetLevelProgress01();

        if (timePlayedText != null)
            timePlayedText.text = $"Time Played: {profileManager.GetFormattedTimePlayed()}";

        if (lifetimePointsText != null)
            lifetimePointsText.text = $"Lifetime Points Earned: {profileManager.LifetimePointsEarned}";
    }
}