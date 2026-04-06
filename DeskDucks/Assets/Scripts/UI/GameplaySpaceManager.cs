using UnityEngine;

public class GameplaySpaceManager : MonoBehaviour
{
    public static GameplaySpaceManager Instance { get; private set; }

    private const string GlobalScaleKey = "Settings_GlobalGameScale";
    private const string GroundOffsetKey = "Settings_GroundOffset";

    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [Header("Local Gameplay View")]
    [Min(0.1f)][SerializeField] private float globalGameScale = 1f;
    [SerializeField] private float groundOffset = 0.35f;
    [SerializeField] private float topPadding = 0f;
    [SerializeField] private float sidePadding = 0f;

    public Camera TargetCamera
    {
        get
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            return targetCamera;
        }
    }

    public float GlobalGameScale => globalGameScale;
    public float GroundOffset => groundOffset;

    public float LeftWorld => ViewportToWorld(0f, 0f).x + sidePadding;
    public float RightWorld => ViewportToWorld(1f, 0f).x - sidePadding;
    public float BottomWorld => ViewportToWorld(0f, 0f).y;
    public float TopWorld => ViewportToWorld(0f, 1f).y - topPadding;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (targetCamera == null)
            targetCamera = Camera.main;

        LoadSettings();
    }

    void OnValidate()
    {
        globalGameScale = Mathf.Max(0.1f, globalGameScale);
    }

    public void SetGlobalGameScale(float value)
    {
        globalGameScale = Mathf.Max(0.1f, value);
        SaveSettings();
    }

    public void SetGroundOffset(float value)
    {
        groundOffset = value;
        SaveSettings();
    }

    public float GetGroundY(float scaledHalfHeight)
    {
        return BottomWorld + scaledHalfHeight + groundOffset;
    }

    public float GetTopY(float scaledHalfHeight)
    {
        return TopWorld - scaledHalfHeight;
    }

    public float GetMinX(float scaledHalfWidth)
    {
        return LeftWorld + scaledHalfWidth;
    }

    public float GetMaxX(float scaledHalfWidth)
    {
        return RightWorld - scaledHalfWidth;
    }

    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Camera cam = TargetCamera;
        if (cam == null)
            return Vector2.zero;

        return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
    }

    public Vector2 GlobalScreenToWorld(Vector2 globalScreenPos)
    {
        float flippedY = Screen.height - globalScreenPos.y;
        return ScreenToWorld(new Vector2(globalScreenPos.x, flippedY));
    }

    public Vector2 WorldToNormalized(Vector2 worldPos, DuckBounds bounds)
    {
        float halfWidth = bounds != null ? bounds.ScaledHalfWidth : 0f;
        float halfHeight = bounds != null ? bounds.ScaledHalfHeight : 0f;

        float minX = GetMinX(halfWidth);
        float maxX = GetMaxX(halfWidth);
        float groundY = GetGroundY(halfHeight);
        float topY = GetTopY(halfHeight);

        float normalizedX = Mathf.InverseLerp(minX, maxX, worldPos.x);
        float normalizedY = Mathf.InverseLerp(groundY, topY, worldPos.y);

        return new Vector2(normalizedX, normalizedY);
    }

    public Vector2 NormalizedToWorld(Vector2 normalizedPos, DuckBounds bounds, bool snapToGround = false)
    {
        float halfWidth = bounds != null ? bounds.ScaledHalfWidth : 0f;
        float halfHeight = bounds != null ? bounds.ScaledHalfHeight : 0f;

        float minX = GetMinX(halfWidth);
        float maxX = GetMaxX(halfWidth);
        float groundY = GetGroundY(halfHeight);
        float topY = GetTopY(halfHeight);

        float x = Mathf.Lerp(minX, maxX, Mathf.Clamp01(normalizedPos.x));
        float y = snapToGround
            ? groundY
            : Mathf.Lerp(groundY, topY, Mathf.Clamp01(normalizedPos.y));

        return new Vector2(x, y);
    }

    void LoadSettings()
    {
        globalGameScale = PlayerPrefs.GetFloat(GlobalScaleKey, globalGameScale);
        globalGameScale = Mathf.Max(0.1f, globalGameScale);

        groundOffset = PlayerPrefs.GetFloat(GroundOffsetKey, groundOffset);
    }

    void SaveSettings()
    {
        PlayerPrefs.SetFloat(GlobalScaleKey, globalGameScale);
        PlayerPrefs.SetFloat(GroundOffsetKey, groundOffset);
        PlayerPrefs.Save();
    }

    Vector3 ViewportToWorld(float x, float y)
    {
        Camera cam = TargetCamera;
        if (cam == null)
            return Vector3.zero;

        return cam.ViewportToWorldPoint(new Vector3(x, y, 0f));
    }
}