using TMPro;
using UnityEngine;

public class TestingSettingsPanelUI : MonoBehaviour
{
    private const string UiScaleKey = "Settings_UIScale";

    [Header("References")]
    [SerializeField] private RectTransform uiRoot;
    [SerializeField] private TMP_Text globalGameSizeValueText;
    [SerializeField] private TMP_Text uiSizeValueText;
    [SerializeField] private TMP_Text groundValueText;

    [Header("Step Settings")]
    [SerializeField] private float globalGameScaleStep = 0.1f;
    [SerializeField] private float uiScaleStep = 0.1f;
    [SerializeField] private float groundOffsetStep = 0.1f;

    [Header("Limits")]
    [SerializeField] private float minGlobalGameScale = 0.5f;
    [SerializeField] private float maxGlobalGameScale = 3f;
    [SerializeField] private float minUiScale = 0.5f;
    [SerializeField] private float maxUiScale = 3f;
    [SerializeField] private float minGroundOffset = -3f;
    [SerializeField] private float maxGroundOffset = 3f;

    private GameplaySpaceManager gameplaySpace;
    private float uiScale = 1f;

    void Awake()
    {
        gameplaySpace = GameplaySpaceManager.Instance;

        if (uiRoot == null)
            uiRoot = transform.root as RectTransform;

        LoadUiScale();
        ApplyUiScale();
        RefreshTexts();
    }

    void OnEnable()
    {
        gameplaySpace = GameplaySpaceManager.Instance;
        RefreshTexts();
    }

    public void IncreaseGlobalGameScale()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        if (gameplaySpace == null)
            return;

        float newValue = gameplaySpace.GlobalGameScale + globalGameScaleStep;
        gameplaySpace.SetGlobalGameScale(Mathf.Clamp(newValue, minGlobalGameScale, maxGlobalGameScale));
        RefreshTexts();
    }

    public void DecreaseGlobalGameScale()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        if (gameplaySpace == null)
            return;

        float newValue = gameplaySpace.GlobalGameScale - globalGameScaleStep;
        gameplaySpace.SetGlobalGameScale(Mathf.Clamp(newValue, minGlobalGameScale, maxGlobalGameScale));
        RefreshTexts();
    }

    public void IncreaseUiScale()
    {
        uiScale = Mathf.Clamp(uiScale + uiScaleStep, minUiScale, maxUiScale);
        ApplyUiScale();
        SaveUiScale();
        RefreshTexts();
    }

    public void DecreaseUiScale()
    {
        uiScale = Mathf.Clamp(uiScale - uiScaleStep, minUiScale, maxUiScale);
        ApplyUiScale();
        SaveUiScale();
        RefreshTexts();
    }

    public void IncreaseGroundOffset()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        if (gameplaySpace == null)
            return;

        float newValue = gameplaySpace.GroundOffset + groundOffsetStep;
        gameplaySpace.SetGroundOffset(Mathf.Clamp(newValue, minGroundOffset, maxGroundOffset));
        RefreshTexts();
    }

    public void DecreaseGroundOffset()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        if (gameplaySpace == null)
            return;

        float newValue = gameplaySpace.GroundOffset - groundOffsetStep;
        gameplaySpace.SetGroundOffset(Mathf.Clamp(newValue, minGroundOffset, maxGroundOffset));
        RefreshTexts();
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void LoadUiScale()
    {
        uiScale = PlayerPrefs.GetFloat(UiScaleKey, 1f);
        uiScale = Mathf.Clamp(uiScale, minUiScale, maxUiScale);
    }

    void SaveUiScale()
    {
        PlayerPrefs.SetFloat(UiScaleKey, uiScale);
        PlayerPrefs.Save();
    }

    void ApplyUiScale()
    {
        if (uiRoot == null)
            return;

        uiRoot.localScale = Vector3.one * uiScale;
    }

    void RefreshTexts()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        if (globalGameSizeValueText != null && gameplaySpace != null)
            globalGameSizeValueText.text = gameplaySpace.GlobalGameScale.ToString("0.0");

        if (uiSizeValueText != null)
            uiSizeValueText.text = uiScale.ToString("0.0");

        if (groundValueText != null && gameplaySpace != null)
            groundValueText.text = gameplaySpace.GroundOffset.ToString("0.0");
    }
    public void HideUI()
    {
        if (uiRoot == null)
            return;

        uiRoot.gameObject.SetActive(false);
    }
}