using UnityEngine;

public class UIRuntimeSettings : MonoBehaviour
{
    public static UIRuntimeSettings Instance { get; private set; }

    private const string UiScaleKey = "Settings_UIScale";
    private const string GlobalGameScaleKey = "Settings_GlobalGameScale";
    private const string GroundOffsetKey = "Settings_GroundOffset";

    [Header("References")]
    [SerializeField] private RectTransform uiRoot;

    [Header("Defaults")]
    [SerializeField] private float defaultUiScale = 1f;
    [SerializeField] private float defaultGlobalGameScale = 1f;
    [SerializeField] private float defaultGroundOffset = 0.35f;

    private float uiScale;
    private float globalGameScale;
    private float groundOffset;

    public float UiScale => uiScale;
    public float GlobalGameScale => globalGameScale;
    public float GroundOffset => groundOffset;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadAll();
        ApplyAll();
    }

    public void SetUiRoot(RectTransform root)
    {
        uiRoot = root;
        ApplyUiScale();
    }

    public void SetUiScale(float value)
    {
        uiScale = value;
        SaveUiScale();
        ApplyUiScale();
    }

    public void SetGlobalGameScale(float value)
    {
        globalGameScale = value;
        SaveGlobalGameScale();
        ApplyGlobalGameScale();
    }

    public void SetGroundOffset(float value)
    {
        groundOffset = value;
        SaveGroundOffset();
        ApplyGroundOffset();
    }

    public void ApplyAll()
    {
        ApplyUiScale();
        ApplyGlobalGameScale();
        ApplyGroundOffset();
    }

    void LoadAll()
    {
        uiScale = PlayerPrefs.GetFloat(UiScaleKey, defaultUiScale);
        globalGameScale = PlayerPrefs.GetFloat(GlobalGameScaleKey, defaultGlobalGameScale);
        groundOffset = PlayerPrefs.GetFloat(GroundOffsetKey, defaultGroundOffset);
    }

    void SaveUiScale()
    {
        PlayerPrefs.SetFloat(UiScaleKey, uiScale);
        PlayerPrefs.Save();
    }

    void SaveGlobalGameScale()
    {
        PlayerPrefs.SetFloat(GlobalGameScaleKey, globalGameScale);
        PlayerPrefs.Save();
    }

    void SaveGroundOffset()
    {
        PlayerPrefs.SetFloat(GroundOffsetKey, groundOffset);
        PlayerPrefs.Save();
    }

    void ApplyUiScale()
    {
        if (uiRoot == null)
            return;

        uiRoot.localScale = Vector3.one * uiScale;
    }

    void ApplyGlobalGameScale()
    {
        GameplaySpaceManager gameplaySpace = GameplaySpaceManager.Instance;
        if (gameplaySpace == null)
            return;

        gameplaySpace.SetGlobalGameScale(globalGameScale);
    }

    void ApplyGroundOffset()
    {
        GameplaySpaceManager gameplaySpace = GameplaySpaceManager.Instance;
        if (gameplaySpace == null)
            return;

        gameplaySpace.SetGroundOffset(groundOffset);
    }
}