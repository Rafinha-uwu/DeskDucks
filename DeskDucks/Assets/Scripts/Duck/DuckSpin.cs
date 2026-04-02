using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
public class DuckSpin : MonoBehaviour
{
    [Header("References")]
    public Transform visualRoot;

    [Header("Spin")]
    public float spinSpeed = 720f;
    public float returnSpeed = 8f;

    private SimpleGravity gravity;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();

        if (visualRoot == null)
            visualRoot = transform.Find("VisualRoot");
    }

    void Update()
    {
        HandleSpin();
    }

    void HandleSpin()
    {
        if (visualRoot == null)
            return;

        if (!gravity.IsGrounded && gravity.Velocity.y > 0f)
        {
            float direction = Mathf.Sign(gravity.Velocity.x);

            if (direction == 0f)
                direction = 1f;

            visualRoot.Rotate(0f, 0f, -direction * spinSpeed * Time.deltaTime);
        }
        else
        {
            visualRoot.localRotation = Quaternion.Lerp(
                visualRoot.localRotation,
                Quaternion.identity,
                returnSpeed * Time.deltaTime
            );
        }
    }
}