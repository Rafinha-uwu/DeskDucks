using UnityEngine;

public class GlobalGameplayScaler : MonoBehaviour
{
    [SerializeField] private bool captureBaseScaleOnAwake = true;
    [SerializeField] private Vector3 baseLocalScale = Vector3.one;

    private GameplaySpaceManager gameplaySpace;
    private float lastAppliedScale = -1f;

    void Awake()
    {
        if (captureBaseScaleOnAwake)
            baseLocalScale = transform.localScale;

        gameplaySpace = GameplaySpaceManager.Instance;
        ApplyScale(true);
    }

    void OnEnable()
    {
        gameplaySpace = GameplaySpaceManager.Instance;

        if (gameplaySpace != null)
            gameplaySpace.OnGlobalGameScaleChanged += HandleScaleChanged;

        ApplyScale(true);
    }

    void OnDisable()
    {
        if (gameplaySpace != null)
            gameplaySpace.OnGlobalGameScaleChanged -= HandleScaleChanged;
    }

    void Update()
    {
        if (gameplaySpace == null)
        {
            gameplaySpace = GameplaySpaceManager.Instance;

            if (gameplaySpace != null)
            {
                gameplaySpace.OnGlobalGameScaleChanged -= HandleScaleChanged;
                gameplaySpace.OnGlobalGameScaleChanged += HandleScaleChanged;
                ApplyScale(true);
            }

            return;
        }

        ApplyScale(false);
    }

    void OnValidate()
    {
        if (!Application.isPlaying && captureBaseScaleOnAwake)
            baseLocalScale = transform.localScale;
    }

    void HandleScaleChanged(float newScale)
    {
        ApplyScale(true);
    }

    void ApplyScale(bool force)
    {
        if (gameplaySpace == null)
            return;

        float scale = gameplaySpace.GlobalGameScale;

        if (!force && Mathf.Approximately(scale, lastAppliedScale))
            return;

        transform.localScale = baseLocalScale * scale;
        lastAppliedScale = scale;
    }
}