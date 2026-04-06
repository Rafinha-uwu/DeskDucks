using UnityEngine;

public class GlobalGameplayScaler : MonoBehaviour
{
    [SerializeField] private bool captureBaseScaleOnAwake = true;
    [SerializeField] private Vector3 baseLocalScale = Vector3.one;

    private GameplaySpaceManager gameplaySpace;
    private float lastAppliedScale = -1f;

    void Awake()
    {
        gameplaySpace = GameplaySpaceManager.Instance;

        if (captureBaseScaleOnAwake)
            baseLocalScale = transform.localScale;

        ApplyScale(true);
    }

    void Update()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        ApplyScale(false);
    }

    void OnValidate()
    {
        if (!Application.isPlaying && captureBaseScaleOnAwake)
            baseLocalScale = transform.localScale;
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