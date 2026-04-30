using FishNet.Component.Transforming;
using FishNet.Object;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class NetworkDuck : NetworkBehaviour
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

    [Header("Optional")]
    [SerializeField] private bool disableCollidersOnNonServer = true;
    [SerializeField] private Collider2D[] collidersToToggle;

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
    }

    private void AutoAssignReferences()
    {
        if (duck == null)
            duck = GetComponent<Duck>();

        if (gravity == null)
            gravity = GetComponent<SimpleGravity>();

        if (wander == null)
            wander = GetComponent<DuckWander>();

        if (sleep == null)
            sleep = GetComponent<DuckSleep>();

        if (quack == null)
            quack = GetComponent<DuckQuack>();

        if (spin == null)
            spin = GetComponent<DuckSpin>();

        if (stateController == null)
            stateController = GetComponent<DuckStateController>();

        if (animationController == null)
            animationController = GetComponent<DuckAnimationController>();

        if (worldBounds == null)
            worldBounds = GetComponent<WorldBounds>();

        if (collidersToToggle == null || collidersToToggle.Length == 0)
            collidersToToggle = GetComponentsInChildren<Collider2D>(true);
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
        SetBehaviourEnabled(worldBounds, shouldSimulate);

        if (disableCollidersOnNonServer)
        {
            for (int i = 0; i < collidersToToggle.Length; i++)
            {
                if (collidersToToggle[i] != null)
                    collidersToToggle[i].enabled = shouldSimulate;
            }
        }
    }

    private void SetBehaviourEnabled(Behaviour behaviour, bool value)
    {
        if (behaviour != null)
            behaviour.enabled = value;
    }
}