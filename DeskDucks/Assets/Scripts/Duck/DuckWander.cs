using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
[RequireComponent(typeof(DuckBounds))]
public class DuckWander : MonoBehaviour
{
    [Header("State Control")]
    public bool enableWander = true;

    [Header("References")]
    [SerializeField] private Transform visualRoot;

    [Header("Walk Speed")]
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private float maxSpeed = 2f;

    [Header("Walk Duration")]
    [SerializeField] private float minWalkTime = 1f;
    [SerializeField] private float maxWalkTime = 4f;

    [Header("Idle Duration")]
    [SerializeField] private float minIdleTime = 0.5f;
    [SerializeField] private float maxIdleTime = 2.5f;

    [Header("Stop Smoothing")]
    [SerializeField] private float stopDeceleration = 4f;

    [Header("Random Direction Changes")]
    [SerializeField] private float directionChangeChancePerSecond = 0.25f;

    [Header("Random Jump Chance")]
    [SerializeField] private float jumpChancePerSecond = 0.12f;

    [Header("Random Jump Height")]
    [SerializeField] private float minJumpForce = 3f;
    [SerializeField] private float maxJumpForce = 6f;

    [Header("Random Jump Forward Boost")]
    [SerializeField] private float minJumpForwardMultiplier = 1.1f;
    [SerializeField] private float maxJumpForwardMultiplier = 1.8f;

    private enum State
    {
        Idle,
        Walking,
        SlowingDown
    }

    private SimpleGravity gravity;
    private DuckBounds duckBounds;
    private GameplaySpaceManager gameplaySpace;

    private State currentState;
    private float stateTimer;
    private float currentSpeed;
    private int direction = 1;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
        duckBounds = GetComponent<DuckBounds>();
        gameplaySpace = GameplaySpaceManager.Instance;

        if (visualRoot == null)
            visualRoot = transform.Find("VisualRoot");
    }

    void Start()
    {
        ResetToIdle();
    }

    void Update()
    {
        if (!enableWander)
            return;

        if (!gravity.IsGrounded)
            return;

        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                if (stateTimer <= 0f)
                    EnterWalkingState();
                break;

            case State.Walking:
                UpdateWalking();
                if (stateTimer <= 0f)
                    EnterSlowingDownState();
                break;

            case State.SlowingDown:
                UpdateSlowingDown();
                break;
        }
    }

    public void SetWanderEnabled(bool value)
    {
        enableWander = value;
    }

    public void ResetToIdle()
    {
        currentState = State.Idle;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);
        currentSpeed = 0f;
        gravity.StopHorizontalMovement();
    }

    void EnterWalkingState()
    {
        currentState = State.Walking;
        stateTimer = Random.Range(minWalkTime, maxWalkTime);
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        direction = Random.value < 0.5f ? -1 : 1;

        FlipVisual(direction);
    }

    void EnterSlowingDownState()
    {
        currentState = State.SlowingDown;
    }

    void UpdateWalking()
    {
        gravity.SetHorizontalVelocity(direction * currentSpeed);

        HandleScreenEdges();
        HandleRandomDirectionChange();
        HandleRandomJump();
        FlipVisual(direction);
    }

    void UpdateSlowingDown()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, stopDeceleration * Time.deltaTime);
        gravity.SetHorizontalVelocity(direction * currentSpeed);
        FlipVisual(direction);

        if (currentSpeed <= 0.01f)
            ResetToIdle();
    }

    void HandleRandomDirectionChange()
    {
        if (Random.value < directionChangeChancePerSecond * Time.deltaTime)
            direction *= -1;
    }

    void HandleRandomJump()
    {
        if (Random.value >= jumpChancePerSecond * Time.deltaTime)
            return;

        float jumpY = Random.Range(minJumpForce, maxJumpForce);
        float jumpForward = currentSpeed * Random.Range(minJumpForwardMultiplier, maxJumpForwardMultiplier);

        gravity.SetVelocity(new Vector2(direction * jumpForward, jumpY));
    }

    void HandleScreenEdges()
    {
        if (gameplaySpace == null || duckBounds == null)
            return;

        float left = gameplaySpace.GetMinX(duckBounds.ScaledHalfWidth);
        float right = gameplaySpace.GetMaxX(duckBounds.ScaledHalfWidth);

        if (transform.position.x <= left)
            direction = 1;
        else if (transform.position.x >= right)
            direction = -1;
    }

    void FlipVisual(int dir)
    {
        if (visualRoot == null || dir == 0)
            return;

        Vector3 scale = visualRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir < 0 ? -1f : 1f);
        visualRoot.localScale = scale;
    }
}