using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetworkDuck : NetworkBehaviour, IClickable, IDraggable
{
    [Header("Simulation Scripts")]
    [SerializeField] private Duck duck;
    [SerializeField] private SimpleGravity gravity;
    [SerializeField] private DuckWander wander;
    [SerializeField] private DuckSleep sleep;
    [SerializeField] private DuckQuack quack;
    [SerializeField] private DuckSpin spin;
    [SerializeField] private DuckStateController stateController;
    [SerializeField] private DuckAnimationController animationController;
    [SerializeField] private WorldBounds worldBounds;

    [Header("Visual Sync")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float visualSyncInterval = 0.05f;

    private GameplaySpaceManager gameplaySpace;
    private float visualSyncTimer;

    private bool isHeld;
    private NetworkConnection holder;

    private void Awake()
    {
        AutoAssignReferences();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ApplyMode();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        ApplyMode();

        if (quack != null)
            quack.SetAwardPointsToLocalManager(false);
    }

    private void Update()
    {
        if (!IsServerInitialized)
            return;

        SyncVisualsTick();
    }

    private void AutoAssignReferences()
    {
        gameplaySpace = GameplaySpaceManager.Instance;

        if (duck == null) duck = GetComponent<Duck>();
        if (gravity == null) gravity = GetComponent<SimpleGravity>();
        if (wander == null) wander = GetComponent<DuckWander>();
        if (sleep == null) sleep = GetComponent<DuckSleep>();
        if (quack == null) quack = GetComponent<DuckQuack>();
        if (spin == null) spin = GetComponent<DuckSpin>();
        if (stateController == null) stateController = GetComponent<DuckStateController>();
        if (animationController == null) animationController = GetComponent<DuckAnimationController>();
        if (worldBounds == null) worldBounds = GetComponent<WorldBounds>();
        if (visualRoot == null) visualRoot = transform.Find("VisualRoot");
    }

    private void ApplyMode()
    {
        bool shouldSimulate = IsServerInitialized;

        SetBehaviourEnabled(duck, shouldSimulate);
        SetBehaviourEnabled(gravity, shouldSimulate);
        SetBehaviourEnabled(wander, shouldSimulate);
        SetBehaviourEnabled(sleep, shouldSimulate);
        SetBehaviourEnabled(quack, shouldSimulate);
        SetBehaviourEnabled(spin, shouldSimulate);
        SetBehaviourEnabled(stateController, shouldSimulate);
        SetBehaviourEnabled(animationController, shouldSimulate);

        SetBehaviourEnabled(worldBounds, true);
    }

    private void SetBehaviourEnabled(Behaviour behaviour, bool value)
    {
        if (behaviour != null)
            behaviour.enabled = value;
    }

    public void OnClick()
    {
        if (IsServerInitialized)
        {
            HandleClickOnServer();
            return;
        }

        RequestClickServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestClickServerRpc(NetworkConnection sender = null)
    {
        HandleClickOnServer();
    }

    private void HandleClickOnServer()
    {
        if (quack == null || gravity == null)
            return;

        if (!gravity.IsGrounded)
            return;

        if (stateController != null && !stateController.CanStartClickQuack)
            return;

        int pointsToAward = quack.GetUpgradePointsValue(UpgradeType.DuckClick);

        quack.TriggerGroundClickQuack();

        if (Owner != null)
            AwardPointsToOwnerTargetRpc(Owner, pointsToAward);
    }

    [TargetRpc]
    private void AwardPointsToOwnerTargetRpc(NetworkConnection target, int amount)
    {
        DuckPointsManager.Instance?.AddPoints(amount);
    }

    public void OnDragStart(Vector2 worldPos)
    {
        Vector2 normalizedPos = WorldToNormalized(worldPos);

        if (IsServerInitialized)
        {
            TryBeginDrag(normalizedPos, null);
            return;
        }

        RequestBeginDragServerRpc(normalizedPos);
    }

    public void OnDrag(Vector2 worldPos)
    {
        Vector2 normalizedPos = WorldToNormalized(worldPos);

        if (IsServerInitialized)
        {
            ApplyDrag(normalizedPos, null);
            return;
        }

        SendDragServerRpc(normalizedPos);
    }

    public void OnDragEnd(Vector2 velocity)
    {
        Vector2 normalizedVelocity = VelocityToNormalized(velocity);

        if (IsServerInitialized)
        {
            EndDrag(normalizedVelocity, null);
            return;
        }

        RequestEndDragServerRpc(normalizedVelocity);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestBeginDragServerRpc(Vector2 normalizedWorldPos, NetworkConnection sender = null)
    {
        TryBeginDrag(normalizedWorldPos, sender);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendDragServerRpc(Vector2 normalizedWorldPos, NetworkConnection sender = null)
    {
        ApplyDrag(normalizedWorldPos, sender);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestEndDragServerRpc(Vector2 normalizedVelocity, NetworkConnection sender = null)
    {
        EndDrag(normalizedVelocity, sender);
    }

    private void TryBeginDrag(Vector2 normalizedWorldPos, NetworkConnection sender)
    {
        if (isHeld)
            return;

        isHeld = true;
        holder = sender;

        Vector2 serverWorldPos = NormalizedToWorld(normalizedWorldPos);
        duck?.OnDragStart(serverWorldPos);
    }

    private void ApplyDrag(Vector2 normalizedWorldPos, NetworkConnection sender)
    {
        if (!CanControl(sender))
            return;

        Vector2 serverWorldPos = NormalizedToWorld(normalizedWorldPos);
        duck?.OnDrag(serverWorldPos);
    }

    private void EndDrag(Vector2 normalizedVelocity, NetworkConnection sender)
    {
        if (!CanControl(sender))
            return;

        Vector2 serverVelocity = NormalizedToVelocity(normalizedVelocity);

        duck?.OnDragEnd(serverVelocity);

        isHeld = false;
        holder = null;
    }

    private bool CanControl(NetworkConnection sender)
    {
        if (!isHeld)
            return false;

        if (holder == null && sender == null)
            return true;

        return holder == sender;
    }

    private Vector2 WorldToNormalized(Vector2 worldPos)
    {
        GameplaySpaceManager space = GetGameplaySpace();
        if (space == null)
            return worldPos;

        return space.WorldToNormalized(worldPos, worldBounds);
    }

    private Vector2 NormalizedToWorld(Vector2 normalizedPos)
    {
        GameplaySpaceManager space = GetGameplaySpace();
        if (space == null)
            return normalizedPos;

        return space.NormalizedToWorld(normalizedPos, worldBounds);
    }

    private Vector2 VelocityToNormalized(Vector2 velocity)
    {
        GameplaySpaceManager space = GetGameplaySpace();
        if (space == null || worldBounds == null)
            return velocity;

        float minX = space.GetMinX(worldBounds.ScaledHalfWidth);
        float maxX = space.GetMaxX(worldBounds.ScaledHalfWidth);
        float groundY = space.GetGroundY(worldBounds.ScaledHalfHeight);
        float topY = space.GetTopY(worldBounds.ScaledHalfHeight);

        float width = Mathf.Max(0.0001f, maxX - minX);
        float height = Mathf.Max(0.0001f, topY - groundY);

        return new Vector2(velocity.x / width, velocity.y / height);
    }

    private Vector2 NormalizedToVelocity(Vector2 normalizedVelocity)
    {
        GameplaySpaceManager space = GetGameplaySpace();
        if (space == null || worldBounds == null)
            return normalizedVelocity;

        float minX = space.GetMinX(worldBounds.ScaledHalfWidth);
        float maxX = space.GetMaxX(worldBounds.ScaledHalfWidth);
        float groundY = space.GetGroundY(worldBounds.ScaledHalfHeight);
        float topY = space.GetTopY(worldBounds.ScaledHalfHeight);

        float width = Mathf.Max(0.0001f, maxX - minX);
        float height = Mathf.Max(0.0001f, topY - groundY);

        return new Vector2(normalizedVelocity.x * width, normalizedVelocity.y * height);
    }

    private GameplaySpaceManager GetGameplaySpace()
    {
        if (gameplaySpace == null)
            gameplaySpace = GameplaySpaceManager.Instance;

        return gameplaySpace;
    }

    private void SyncVisualsTick()
    {
        if (visualRoot == null)
            return;

        visualSyncTimer -= Time.deltaTime;
        if (visualSyncTimer > 0f)
            return;

        visualSyncTimer = visualSyncInterval;

        float visualRotationZ = visualRoot.localEulerAngles.z;
        float visualScaleX = visualRoot.localScale.x;

        ApplyVisualsObserversRpc(visualRotationZ, visualScaleX);
    }

    [ObserversRpc(BufferLast = true)]
    private void ApplyVisualsObserversRpc(float visualRotationZ, float visualScaleX)
    {
        if (IsServerInitialized)
            return;

        if (visualRoot == null)
            visualRoot = transform.Find("VisualRoot");

        if (visualRoot == null)
            return;

        Vector3 euler = visualRoot.localEulerAngles;
        euler.z = visualRotationZ;
        visualRoot.localEulerAngles = euler;

        Vector3 scale = visualRoot.localScale;
        scale.x = visualScaleX;
        visualRoot.localScale = scale;
    }
}