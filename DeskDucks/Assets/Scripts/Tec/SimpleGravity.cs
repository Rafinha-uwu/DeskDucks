using System;
using UnityEngine;

[RequireComponent(typeof(WorldBounds))]
public class SimpleGravity : MonoBehaviour
{
    public enum BounceType
    {
        SideWall,
        Ceiling,
        Ground
    }

    public event Action<BounceType, float> OnBounce;

    [Header("Physics")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float friction = 6f;

    [Header("Wall / Ceiling Bounce")]
    [SerializeField] private float sideBounceMultiplier = 0.8f;
    [SerializeField] private float topBounceMultiplier = 0.5f;

    [Header("Ground Bounce")]
    [SerializeField] private bool enableGroundBounce = true;
    [SerializeField] private float groundBounceMultiplier = 0.18f;
    [SerializeField] private float minFallSpeedForGroundBounce = 2.5f;

    private Vector2 velocity;
    private WorldBounds worldBounds;
    private GameplaySpaceManager gameplaySpace;

    private bool isGrounded;
    private bool canGroundBounce = true;

    public Vector2 Velocity => velocity;
    public bool IsGrounded => isGrounded;

    void Awake()
    {
        worldBounds = GetComponent<WorldBounds>();
        gameplaySpace = GameplaySpaceManager.Instance;
    }

    void Update()
    {
        if (worldBounds == null)
            return;

        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        if (gameplaySpace == null)
            return;

        velocity.y += gravity * Time.deltaTime;

        Vector3 pos = transform.position + (Vector3)(velocity * Time.deltaTime);

        float minX = gameplaySpace.GetMinX(worldBounds.ScaledHalfWidth);
        float maxX = gameplaySpace.GetMaxX(worldBounds.ScaledHalfWidth);
        float groundY = gameplaySpace.GetGroundY(worldBounds.ScaledHalfHeight);
        float topY = gameplaySpace.GetTopY(worldBounds.ScaledHalfHeight);

        isGrounded = false;

        if (pos.x < minX)
        {
            float impactSpeed = Mathf.Abs(velocity.x);
            pos.x = minX;

            if (velocity.x < 0f)
            {
                velocity.x = -velocity.x * sideBounceMultiplier;
                OnBounce?.Invoke(BounceType.SideWall, impactSpeed);
            }
        }
        else if (pos.x > maxX)
        {
            float impactSpeed = Mathf.Abs(velocity.x);
            pos.x = maxX;

            if (velocity.x > 0f)
            {
                velocity.x = -velocity.x * sideBounceMultiplier;
                OnBounce?.Invoke(BounceType.SideWall, impactSpeed);
            }
        }

        if (pos.y > topY)
        {
            float impactSpeed = Mathf.Abs(velocity.y);
            pos.y = topY;

            if (velocity.y > 0f)
            {
                velocity.y = -velocity.y * topBounceMultiplier;
                OnBounce?.Invoke(BounceType.Ceiling, impactSpeed);
            }
        }

        if (pos.y <= groundY)
        {
            pos.y = groundY;

            bool shouldGroundBounce =
                enableGroundBounce &&
                canGroundBounce &&
                velocity.y < -minFallSpeedForGroundBounce;

            if (shouldGroundBounce)
            {
                float impactSpeed = Mathf.Abs(velocity.y);
                velocity.y = -velocity.y * groundBounceMultiplier;
                canGroundBounce = false;
                OnBounce?.Invoke(BounceType.Ground, impactSpeed);
            }
            else
            {
                velocity.y = 0f;
                isGrounded = true;
                velocity.x = Mathf.Lerp(velocity.x, 0f, friction * Time.deltaTime);
            }
        }
        else
        {
            canGroundBounce = true;
        }

        transform.position = pos;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
        isGrounded = false;
    }

    public void SetHorizontalVelocity(float x)
    {
        velocity.x = x;
    }

    public void SetVerticalVelocity(float y)
    {
        velocity.y = y;
        isGrounded = false;
    }

    public void StopHorizontalMovement()
    {
        velocity.x = 0f;
    }

    public void ResetVelocity()
    {
        velocity = Vector2.zero;
        isGrounded = false;
    }

    public void SetGravityEnabled(bool value)
    {
        enabled = value;
    }
}