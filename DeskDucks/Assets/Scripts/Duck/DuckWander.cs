using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
[RequireComponent(typeof(WorldBounds))]
public class DuckWander : MonoBehaviour
{
    private enum State
    {
        Idle,
        Walking,
        SlowingDown
    }

    [Header("State Control")]
    [SerializeField] private bool enableWander = true;

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

    private SimpleGravity gravity;
    private WorldBounds worldBounds;
    private GameplaySpaceManager gameplaySpace;
    private DuckStateController stateController;

    private State currentState;
    private float stateTimer;
    private float currentSpeed;
    private int direction = 1;

    public bool IsWanderEnabled => enableWander;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
        worldBounds = GetComponent<WorldBounds>();
        gameplaySpace = GameplaySpaceManager.Instance;
        stateController = GetComponent<DuckStateController>();

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

        if (gameplaySpace == null)
            return;

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

        if (!enableWander)
            gravity.StopHorizontalMovement();
    }

    public void ResetToIdle()
    {
        currentState = State.Idle;
        stateTimer = Random.Range(minIdleTime, maxIdleTime);
        currentSpeed = 0f;
        gravity.StopHorizontalMovement();
        stateController?.SetStateImmediate(DuckStateController.DuckState.Idle);
    }

    public void StartSlowDownFromDirection(float moveDirection, float moveSpeed)
    {
        direction = moveDirection < 0f ? -1 : 1;
        currentSpeed = Mathf.Max(0f, moveSpeed);
        currentState = State.SlowingDown;

        FlipVisual(direction);

        if (currentSpeed <= 0.01f)
            ResetToIdle();
    }

    void EnterWalkingState()
    {
        currentState = State.Walking;
        stateTimer = Random.Range(minWalkTime, maxWalkTime);
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        direction = Random.value < 0.5f ? -1 : 1;

        FlipVisual(direction);
        stateController?.SetStateImmediate(DuckStateController.DuckState.Walking);
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
        stateController?.SetStateImmediate(DuckStateController.DuckState.Airborne);
    }

    void HandleScreenEdges()
    {
        if (gameplaySpace == null || worldBounds == null)
            return;

        float left = gameplaySpace.GetMinX(worldBounds.ScaledHalfWidth);
        float right = gameplaySpace.GetMaxX(worldBounds.ScaledHalfWidth);

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