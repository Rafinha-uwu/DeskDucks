using UnityEngine;

[RequireComponent(typeof(DuckBounds))]
public class SimpleGravity : MonoBehaviour
{
    [Header("Physics")]
    public float gravity = -20f;
    public float groundOffset = 0.35f;
    public float friction = 6f;

    [Header("Wall / Ceiling Bounce")]
    public float sideBounceMultiplier = 0.8f;
    public float topBounceMultiplier = 0.5f;

    [Header("Ground Bounce")]
    public bool enableGroundBounce = true;
    public float groundBounceMultiplier = 0.18f;
    public float minFallSpeedForGroundBounce = 2.5f;

    private Vector2 velocity;
    private Camera mainCamera;
    private DuckBounds duckBounds;

    private bool isGrounded;
    private bool canGroundBounce = true;

    public Vector2 Velocity => velocity;
    public bool IsGrounded => isGrounded;

    void Awake()
    {
        mainCamera = Camera.main;
        duckBounds = GetComponent<DuckBounds>();
    }

    void Update()
    {
        velocity.y += gravity * Time.deltaTime;

        Vector3 pos = transform.position + (Vector3)(velocity * Time.deltaTime);

        float minX = GetMinX();
        float maxX = GetMaxX();
        float groundY = GetGroundY();
        float topY = GetTopY();

        isGrounded = false;

        if (pos.x < minX)
        {
            pos.x = minX;

            if (velocity.x < 0f)
                velocity.x = -velocity.x * sideBounceMultiplier;
        }

        if (pos.x > maxX)
        {
            pos.x = maxX;

            if (velocity.x > 0f)
                velocity.x = -velocity.x * sideBounceMultiplier;
        }

        if (pos.y > topY)
        {
            pos.y = topY;

            if (velocity.y > 0f)
                velocity.y = -velocity.y * topBounceMultiplier;
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
                velocity.y = -velocity.y * groundBounceMultiplier;
                canGroundBounce = false;
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

    float GetGroundY()
    {
        float bottom = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;
        return bottom + duckBounds.halfHeight + groundOffset;
    }

    float GetTopY()
    {
        float top = mainCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y;
        return top - duckBounds.halfHeight;
    }

    float GetMinX()
    {
        float left = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        return left + duckBounds.halfWidth;
    }

    float GetMaxX()
    {
        float right = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;
        return right - duckBounds.halfWidth;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
        isGrounded = false;
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