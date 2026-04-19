using UnityEngine;

[RequireComponent(typeof(DuckWander))]
[RequireComponent(typeof(SimpleGravity))]
public class DuckQuack : MonoBehaviour
{
    [Header("Click Quack")]
    [SerializeField] private float clickQuackDuration = 1f;

    [Header("Random Quack")]
    [SerializeField] private bool enableRandomQuacks = true;
    [SerializeField] private float minRandomQuackDelay = 5f;
    [SerializeField] private float maxRandomQuackDelay = 20f;

    [Header("Bounce Points")]
    [SerializeField] private bool enableBouncePoints = true;
    [SerializeField] private float minImpactSpeedForBouncePoints = 1.5f;

    private DuckWander wander;
    private SimpleGravity gravity;
    private DuckStateController stateController;

    private float clickPauseTimer;
    private float randomQuackTimer;

    public bool IsBusy => clickPauseTimer > 0f;

    void Awake()
    {
        wander = GetComponent<DuckWander>();
        gravity = GetComponent<SimpleGravity>();
        stateController = GetComponent<DuckStateController>();
        ResetRandomQuackTimer();
    }

    void OnEnable()
    {
        if (gravity != null)
            gravity.OnBounce += HandleBounce;
    }

    void OnDisable()
    {
        if (gravity != null)
            gravity.OnBounce -= HandleBounce;
    }

    void Update()
    {
        HandleClickPause();
        HandleRandomQuack();
    }

    public void TriggerGroundClickQuack()
    {
        if (wander == null)
            return;

        wander.SetWanderEnabled(false);
        wander.ResetToIdle();

        clickPauseTimer = clickQuackDuration;
        DuckPointsManager.Instance?.AddPoints(GetUpgradePoints(UpgradeType.DuckClick));
        stateController?.SetStateImmediate(DuckStateController.DuckState.ClickQuack);
    }

    void HandleClickPause()
    {
        if (clickPauseTimer <= 0f)
            return;

        clickPauseTimer -= Time.deltaTime;

        if (clickPauseTimer > 0f)
            return;

        clickPauseTimer = 0f;
        wander.ResetToIdle();
        wander.SetWanderEnabled(true);
        ResetRandomQuackTimer();
    }

    void HandleRandomQuack()
    {
        if (!enableRandomQuacks)
            return;

        if (!wander.IsWanderEnabled || clickPauseTimer > 0f)
            return;

        randomQuackTimer -= Time.deltaTime;

        if (randomQuackTimer > 0f)
            return;

        TriggerRandomQuack();
        ResetRandomQuackTimer();
    }

    void TriggerRandomQuack()
    {
        DuckPointsManager.Instance?.AddPoints(GetUpgradePoints(UpgradeType.RandomQuack));
    }

    void HandleBounce(SimpleGravity.BounceType bounceType, float impactSpeed)
    {
        if (!enableBouncePoints)
            return;

        if (gravity == null || gravity.IsGrounded)
            return;

        if (wander != null && wander.IsWanderEnabled && gravity.Velocity.y <= 0.01f)
            return;

        if (impactSpeed < minImpactSpeedForBouncePoints)
            return;

        if (bounceType == SimpleGravity.BounceType.SideWall ||
            bounceType == SimpleGravity.BounceType.Ceiling ||
            bounceType == SimpleGravity.BounceType.Ground)
        {
            DuckPointsManager.Instance?.AddPoints(GetUpgradePoints(UpgradeType.WallBounce));
        }
    }

    int GetUpgradePoints(UpgradeType type)
    {
        if (DuckUpgradeManager.Instance == null)
            return 1;

        return DuckUpgradeManager.Instance.GetCurrentPointsFor(type);
    }

    void ResetRandomQuackTimer()
    {
        randomQuackTimer = Random.Range(minRandomQuackDelay, maxRandomQuackDelay);
    }
}