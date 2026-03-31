using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
public class DuckSpin : MonoBehaviour
{
    public float spinSpeed = 720f; // degrees per second
    public float returnSpeed = 8f;

    private SimpleGravity gravity;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
    }

    void Update()
    {
        HandleSpin();
    }

    void HandleSpin()
    {
        // ?? SPIN while going UP and not grounded
        if (!gravity.IsGrounded && gravity.Velocity.y > 0f)
        {
            float direction = Mathf.Sign(gravity.Velocity.x);

            // if no horizontal movement, default spin direction
            if (direction == 0f) direction = 1f;

            transform.Rotate(0f, 0f, -direction * spinSpeed * Time.deltaTime);
        }
        else
        {
            // ?? Return to upright (0 rotation)
            Quaternion target = Quaternion.identity;

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                target,
                returnSpeed * Time.deltaTime
            );
        }
    }
}