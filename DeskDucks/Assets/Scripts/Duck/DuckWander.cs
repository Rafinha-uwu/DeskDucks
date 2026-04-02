using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
[RequireComponent(typeof(DuckBounds))]
public class DuckWander : MonoBehaviour
{
    [Header("State Control")]
    public bool enableWander = true;

    [Header("References")]
    public Transform visualRoot;

    [Header("Walk Speed")]
    public float minSpeed = 0.5f;
    public float maxSpeed = 2f;

    [Header("Walk Duration")]
    public float minWalkTime = 1f;
    public float maxWalkTime = 4f;

    [Header("Idle Duration")]
    public float minIdleTime = 0.5f;
    public float maxIdleTime = 2.5f;

    [Header("Stop Smoothing")]
    public float stopDeceleration = 4f;

    [Header("Random Direction Changes")]
    public float directionChangeChancePerSecond = 0.25f;

    [Header("Random Jump Chance")]
    public float jumpChancePerSecond = 0.12f;

    [Header("Random Jump Height")]
    public float minJumpForce = 3f;
    public float maxJumpForce = 6f;

    [Header("Random Jump Forward Boost")]
    public float minJumpForwardMultiplier = 1.1f;
    public float maxJumpForwardMultiplier = 1.8f;

    private SimpleGravity gravity;
    private DuckBounds duckBounds;
    private Camera mainCamera;

    private float timer;
    private float currentSpeed;
    private int direction;

    private State currentState;

    private enum State
    {
        Idle,
        Walking,
        SlowingDown
    }

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
        duckBounds = GetComponent<DuckBounds>();
        mainCamera = Camera.main;

        if (visualRoot == null)
            visualRoot = transform.Find("VisualRoot");
    }

    void Start()
    {
        ForceIdle();
    }

    void Update()
    {
        if (!enableWander)
            return;

        if (!gravity.IsGrounded)
            return;

        timer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (timer <= 0f)
                    PickWalk();
                break;

            case State.Walking:
                HandleMovement();
                HandleRandomDirectionChange();
                HandleRandomJump();

                if (timer <= 0f)
                    BeginSlowDown();
                break;

            case State.SlowingDown:
                HandleSlowDown();
                break;
        }
    }

    void HandleMovement()
    {
        Vector2 vel = gravity.Velocity;
        vel.x = direction * currentSpeed;
        gravity.SetVelocity(vel);

        FlipVisual(direction);
        HandleScreenEdges();

        // FUTURE: walking animation speed here
        // animator.speed = currentSpeed;
    }

    void HandleSlowDown()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, stopDeceleration * Time.deltaTime);

        Vector2 vel = gravity.Velocity;
        vel.x = direction * currentSpeed;
        gravity.SetVelocity(vel);

        FlipVisual(direction);

        if (currentSpeed <= 0.01f)
            PickIdleImmediate();
    }

    void HandleRandomDirectionChange()
    {
        if (Random.value < directionChangeChancePerSecond * Time.deltaTime)
        {
            direction *= -1;
            FlipVisual(direction);
        }
    }

    void HandleRandomJump()
    {
        if (Random.value >= jumpChancePerSecond * Time.deltaTime)
            return;

        Vector2 vel = gravity.Velocity;

        float jumpY = Random.Range(minJumpForce, maxJumpForce);
        float jumpForward = currentSpeed * Random.Range(minJumpForwardMultiplier, maxJumpForwardMultiplier);

        vel.x = direction * jumpForward;
        vel.y = jumpY;

        gravity.SetVelocity(vel);

        // FUTURE: trigger jump animation here
    }

    void HandleScreenEdges()
    {
        float left = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float right = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f)).x;

        if (transform.position.x <= left + duckBounds.halfWidth)
        {
            direction = 1;
            FlipVisual(direction);
        }
        else if (transform.position.x >= right - duckBounds.halfWidth)
        {
            direction = -1;
            FlipVisual(direction);
        }
    }

    void PickWalk()
    {
        currentState = State.Walking;
        timer = Random.Range(minWalkTime, maxWalkTime);
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        direction = Random.value < 0.5f ? -1 : 1;

        FlipVisual(direction);

        // FUTURE: enter walking animation here
    }

    void BeginSlowDown()
    {
        currentState = State.SlowingDown;
    }

    void PickIdleImmediate()
    {
        currentState = State.Idle;
        timer = Random.Range(minIdleTime, maxIdleTime);

        Vector2 vel = gravity.Velocity;
        vel.x = 0f;
        gravity.SetVelocity(vel);

        // FUTURE: enter idle animation here
    }

    void FlipVisual(int dir)
    {
        if (visualRoot == null || dir == 0)
            return;

        Vector3 scale = visualRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir < 0 ? -1f : 1f);
        visualRoot.localScale = scale;
    }

    public void ForceIdle()
    {
        currentState = State.Idle;
        timer = Random.Range(minIdleTime, maxIdleTime);
        currentSpeed = 0f;

        Vector2 vel = gravity.Velocity;
        vel.x = 0f;
        gravity.SetVelocity(vel);
    }

    // FUTURE:
    // Add more default random actions here:
    // - sleep
    // - quack
    // - look around
    // - peck ground
}