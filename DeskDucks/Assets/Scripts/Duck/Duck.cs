using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
public class Duck : MonoBehaviour, IClickable, IDraggable
{
    private SimpleGravity gravity;
    private DuckWander wander;
    private DuckQuack quack;

    private Vector3 dragOffset;
    private bool waitingForLandingReset;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
        wander = GetComponent<DuckWander>();
        quack = GetComponent<DuckQuack>();
    }

    void Update()
    {
        if (!waitingForLandingReset)
            return;

        if (!gravity.IsGrounded)
            return;

        waitingForLandingReset = false;

        if (wander != null)
        {
            wander.ForceIdle();
            wander.enableWander = true;
        }
    }

    public void OnClick()
    {
        if (quack == null)
            return;

        if (gravity.IsGrounded)
            quack.TriggerGroundClickQuack();
        else
            quack.TriggerAirClickQuack();
    }

    public void OnDragStart(Vector2 worldPos)
    {
        dragOffset = transform.position - (Vector3)worldPos;

        gravity.ResetVelocity();
        gravity.SetGravityEnabled(false);

        waitingForLandingReset = false;

        if (wander != null)
            wander.enableWander = false;
    }

    public void OnDrag(Vector2 worldPos)
    {
        transform.position = worldPos + (Vector2)dragOffset;
    }

    public void OnDragEnd(Vector2 velocity)
    {
        gravity.SetGravityEnabled(true);
        gravity.SetVelocity(velocity);

        waitingForLandingReset = true;
    }
}