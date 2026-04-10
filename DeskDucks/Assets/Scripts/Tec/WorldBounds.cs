using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    [SerializeField] private float baseHalfWidth = 0.5f;
    [SerializeField] private float baseHalfHeight = 0.5f;

    private Vector3 lastLossyScale;
    private float scaledHalfWidth;
    private float scaledHalfHeight;

    public float ScaledHalfWidth => scaledHalfWidth;
    public float ScaledHalfHeight => scaledHalfHeight;

    void Awake()
    {
        RefreshIfNeeded(true);
    }

    void LateUpdate()
    {
        RefreshIfNeeded(false);
    }

    void RefreshIfNeeded(bool force)
    {
        Vector3 currentScale = transform.lossyScale;

        if (!force && currentScale == lastLossyScale)
            return;

        lastLossyScale = currentScale;
        scaledHalfWidth = baseHalfWidth * Mathf.Abs(currentScale.x);
        scaledHalfHeight = baseHalfHeight * Mathf.Abs(currentScale.y);
    }
}