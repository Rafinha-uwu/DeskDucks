using UnityEngine;

public class DuckProfileManager : MonoBehaviour
{
    public static DuckProfileManager Instance { get; private set; }

    private const string DuckNameKey = "DuckProfile_Name";
    private const string TimePlayedKey = "DuckProfile_TimePlayedSeconds";

    [Header("Defaults")]
    [SerializeField] private string defaultDuckName = "Duck";

    private string duckName;
    private float timePlayedSeconds;

    public string DuckName => duckName;
    public float TimePlayedSeconds => timePlayedSeconds;

    public int CurrentLevel
    {
        get
        {
            int lifetime = DuckPointsManager.Instance != null
                ? DuckPointsManager.Instance.LifetimePointsEarned
                : 0;

            return GetLevelFromLifetimePoints(lifetime);
        }
    }

    public int LifetimePointsEarned
    {
        get
        {
            return DuckPointsManager.Instance != null
                ? DuckPointsManager.Instance.LifetimePointsEarned
                : 0;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadData();
    }

    void Update()
    {
        timePlayedSeconds += Time.deltaTime;
    }

    void OnApplicationQuit()
    {
        SaveData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveData();
    }

    public void SetDuckName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return;

        duckName = newName.Trim();
        SaveData();
    }

    public int GetCurrentLevelStartRequirement()
    {
        int level = CurrentLevel;
        return GetLifetimeRequirementForLevel(level);
    }

    public int GetNextLevelRequirement()
    {
        int level = CurrentLevel;
        return GetLifetimeRequirementForLevel(level + 1);
    }

    public float GetLevelProgress01()
    {
        int lifetime = LifetimePointsEarned;
        int currentStart = GetCurrentLevelStartRequirement();
        int nextReq = GetNextLevelRequirement();

        if (nextReq <= currentStart)
            return 1f;

        return Mathf.Clamp01((float)(lifetime - currentStart) / (nextReq - currentStart));
    }

    public string GetFormattedTimePlayed()
    {
        int totalMinutes = Mathf.FloorToInt(timePlayedSeconds / 60f);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        return $"{hours:00}:{minutes:00}";
    }

    int GetLevelFromLifetimePoints(int lifetimePoints)
    {
        int level = 1;

        while (lifetimePoints >= GetLifetimeRequirementForLevel(level + 1))
            level++;

        return level;
    }

    int GetLifetimeRequirementForLevel(int level)
    {
        if (level <= 1)
            return 0;

        int power = level - 2;
        return (int)Mathf.Pow(10f, power) * 1000;
    }

    void LoadData()
    {
        duckName = PlayerPrefs.GetString(DuckNameKey, defaultDuckName);
        timePlayedSeconds = PlayerPrefs.GetFloat(TimePlayedKey, 0f);
    }

    void SaveData()
    {
        PlayerPrefs.SetString(DuckNameKey, duckName);
        PlayerPrefs.SetFloat(TimePlayedKey, timePlayedSeconds);
        PlayerPrefs.Save();
    }
}