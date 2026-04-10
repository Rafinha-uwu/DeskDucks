using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
public class Duck : MonoBehaviour, IClickable, IDraggable
{
    private SimpleGravity gravity;
    private DuckWander wander;
    private DuckQuack quack;
    private DuckStateController stateController;

    private Vector3 dragOffset;
    private bool waitingForLandingReset;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
        wander = GetComponent<DuckWander>();
        quack = GetComponent<DuckQuack>();
        stateController = GetComponent<DuckStateController>();
    }

    void Update()
    {
        if (gravity == null)
            return;

        if (!gravity.IsGrounded &&
            stateController != null &&
            stateController.CurrentState != DuckStateController.DuckState.Dragged &&
            stateController.CurrentState != DuckStateController.DuckState.ClickQuack)
        {
            stateController.SetStateImmediate(DuckStateController.DuckState.Airborne);
        }

        if (!waitingForLandingReset)
            return;

        if (!gravity.IsGrounded)
            return;

        waitingForLandingReset = false;

        if (wander != null)
        {
            wander.ResetToIdle();
            wander.SetWanderEnabled(true);
        }
    }

    public void OnClick()
    {
        if (quack == null || gravity == null)
            return;

        if (!gravity.IsGrounded)
            return;

        if (stateController != null && !stateController.CanStartClickQuack)
            return;

        quack.TriggerGroundClickQuack();
    }

    public void OnDragStart(Vector2 worldPos)
    {
        if (stateController != null && !stateController.CanStartDrag)
            return;

        dragOffset = transform.position - (Vector3)worldPos;

        gravity.ResetVelocity();
        gravity.SetGravityEnabled(false);
        waitingForLandingReset = false;

        if (wander != null)
            wander.SetWanderEnabled(false);

        stateController?.SetStateImmediate(DuckStateController.DuckState.Dragged);
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

        stateController?.SetStateImmediate(DuckStateController.DuckState.Airborne);
    }
}