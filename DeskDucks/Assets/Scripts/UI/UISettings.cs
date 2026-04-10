using TMPro;
using UnityEngine;

public class UISettings : MonoBehaviour
{
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
    private UIRuntimeSettings runtimeSettings;

    void Awake()
    {
        gameplaySpace = GameplaySpaceManager.Instance;
        runtimeSettings = UIRuntimeSettings.Instance;

        if (uiRoot == null)
            uiRoot = transform.root as RectTransform;

        if (runtimeSettings != null && uiRoot != null)
            runtimeSettings.SetUiRoot(uiRoot);

        RefreshTexts();
    }

    void OnEnable()
    {
        gameplaySpace = GameplaySpaceManager.Instance;
        runtimeSettings = UIRuntimeSettings.Instance;

        if (runtimeSettings != null && uiRoot != null)
            runtimeSettings.SetUiRoot(uiRoot);

        if (runtimeSettings != null)
            runtimeSettings.ApplyAll();

        RefreshTexts();
    }

    public void IncreaseGlobalGameScale()
    {
        if (runtimeSettings == null)
            runtimeSettings = UIRuntimeSettings.Instance;

        if (runtimeSettings == null)
            return;

        float newValue = Mathf.Clamp(
            runtimeSettings.GlobalGameScale + globalGameScaleStep,
            minGlobalGameScale,
            maxGlobalGameScale
        );

        runtimeSettings.SetGlobalGameScale(newValue);
        RefreshTexts();
    }

    public void DecreaseGlobalGameScale()
    {
        if (runtimeSettings == null)
            runtimeSettings = UIRuntimeSettings.Instance;

        if (runtimeSettings == null)
            return;

        float newValue = Mathf.Clamp(
            runtimeSettings.GlobalGameScale - globalGameScaleStep,
            minGlobalGameScale,
            maxGlobalGameScale
        );

        runtimeSettings.SetGlobalGameScale(newValue);
        RefreshTexts();
    }

    public void IncreaseUiScale()
    {
        if (runtimeSettings == null)
            runtimeSettings = UIRuntimeSettings.Instance;

        if (runtimeSettings == null)
            return;

        float newValue = Mathf.Clamp(
            runtimeSettings.UiScale + uiScaleStep,
            minUiScale,
            maxUiScale
        );

        runtimeSettings.SetUiScale(newValue);
        RefreshTexts();
    }

    public void DecreaseUiScale()
    {
        if (runtimeSettings == null)
            runtimeSettings = UIRuntimeSettings.Instance;

        if (runtimeSettings == null)
            return;

        float newValue = Mathf.Clamp(
            runtimeSettings.UiScale - uiScaleStep,
            minUiScale,
            maxUiScale
        );

        runtimeSettings.SetUiScale(newValue);
        RefreshTexts();
    }

    public void IncreaseGroundOffset()
    {
        if (runtimeSettings == null)
            runtimeSettings = UIRuntimeSettings.Instance;

        if (runtimeSettings == null)
            return;

        float newValue = Mathf.Clamp(
            runtimeSettings.GroundOffset + groundOffsetStep,
            minGroundOffset,
            maxGroundOffset
        );

        runtimeSettings.SetGroundOffset(newValue);
        RefreshTexts();
    }

    public void DecreaseGroundOffset()
    {
        if (runtimeSettings == null)
            runtimeSettings = UIRuntimeSettings.Instance;

        if (runtimeSettings == null)
            return;

        float newValue = Mathf.Clamp(
            runtimeSettings.GroundOffset - groundOffsetStep,
            minGroundOffset,
            maxGroundOffset
        );

        runtimeSettings.SetGroundOffset(newValue);
        RefreshTexts();
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void HideUI()
    {
        if (uiRoot == null)
            return;

        uiRoot.gameObject.SetActive(false);
    }

    void RefreshTexts()
    {
        if (runtimeSettings == null)
            runtimeSettings = UIRuntimeSettings.Instance;

        if (globalGameSizeValueText != null && runtimeSettings != null)
            globalGameSizeValueText.text = runtimeSettings.GlobalGameScale.ToString("0.0");

        if (uiSizeValueText != null && runtimeSettings != null)
            uiSizeValueText.text = runtimeSettings.UiScale.ToString("0.0");

        if (groundValueText != null && runtimeSettings != null)
            groundValueText.text = runtimeSettings.GroundOffset.ToString("0.0");
    }
}