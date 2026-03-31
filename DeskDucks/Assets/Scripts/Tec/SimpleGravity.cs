using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleGravity : MonoBehaviour
{
    public float gravity = -20f;
    public float groundOffset = 0.35f;
    public float friction = 6f;

    [Header("Bounce")]
    public float sideBounceMultiplier = 0.8f;
    public float topBounceMultiplier = 0.5f;

    private Vector2 velocity;
    private SpriteRenderer sr;
    private bool isGrounded;

    public Vector2 Velocity => velocity;
    public bool IsGrounded => isGrounded;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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

        // LEFT WALL
        if (pos.x < minX)
        {
            pos.x = minX;

            if (velocity.x < 0f)
                velocity.x = -velocity.x * sideBounceMultiplier;
        }

        // RIGHT WALL
        if (pos.x > maxX)
        {
            pos.x = maxX;

            if (velocity.x > 0f)
                velocity.x = -velocity.x * sideBounceMultiplier;
        }

        // TOP
        if (pos.y > topY)
        {
            pos.y = topY;

            if (velocity.y > 0f)
                velocity.y = -velocity.y * topBounceMultiplier;
        }

        // GROUND
        if (pos.y <= groundY)
        {
            pos.y = groundY;
            velocity.y = 0f;
            isGrounded = true;

            velocity.x = Mathf.Lerp(velocity.x, 0f, friction * Time.deltaTime);
        }

        transform.position = pos;
    }

    float GetGroundY()
    {
        float bottom = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;
        return bottom + sr.bounds.extents.y + groundOffset;
    }

    float GetTopY()
    {
        float top = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y;
        return top - sr.bounds.extents.y;
    }

    float GetMinX()
    {
        float left = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        return left + sr.bounds.extents.x;
    }

    float GetMaxX()
    {
        float right = Camera.main.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;
        return right - sr.bounds.extents.x;
    }

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }

    public void ResetVelocity()
    {
        velocity = Vector2.zero;
    }

    public void SetEnabled(bool value)
    {
        enabled = value;
    }
}