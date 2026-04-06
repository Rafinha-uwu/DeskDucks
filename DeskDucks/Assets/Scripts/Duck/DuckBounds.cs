using UnityEngine;

public class DuckBounds : MonoBehaviour
{
    [Header("Base Gameplay Bounds")]
    [SerializeField] private float baseHalfWidth = 0.5f;
    [SerializeField] private float baseHalfHeight = 0.5f;

    private GameplaySpaceManager gameplaySpace;

    public float BaseHalfWidth => baseHalfWidth;
    public float BaseHalfHeight => baseHalfHeight;

    public float ScaledHalfWidth
    {
        get
        {
            float scale = gameplaySpace != null ? gameplaySpace.GlobalGameScale : 1f;
            return baseHalfWidth * scale;
        }
    }

    public float ScaledHalfHeight
    {
        get
        {
            float scale = gameplaySpace != null ? gameplaySpace.GlobalGameScale : 1f;
            return baseHalfHeight * scale;
        }
    }

    void Awake()
    {
        gameplaySpace = GameplaySpaceManager.Instance;
    }

    void Update()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;
    }
}