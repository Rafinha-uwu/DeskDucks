using UnityEngine;

[RequireComponent(typeof(DuckWander))]
[RequireComponent(typeof(SimpleGravity))]
public class DuckQuack : MonoBehaviour
{
    [Header("Click Quack")]
    [SerializeField] private float clickQuackDuration = 1f;
    [SerializeField] private int clickQuackPoints = 1;

    [Header("Random Quack")]
    [SerializeField] private bool enableRandomQuacks = true;
    [SerializeField] private float minRandomQuackDelay = 5f;
    [SerializeField] private float maxRandomQuackDelay = 20f;
    [SerializeField] private int randomQuackPoints = 1;

    [Header("Bounce Points")]
    [SerializeField] private bool enableBouncePoints = true;

    private DuckWander wander;
    private SimpleGravity gravity;

    private float clickPauseTimer;
    private float randomQuackTimer;

    public bool IsBusy => clickPauseTimer > 0f;

    void Awake()
    {
        wander = GetComponent<DuckWander>();
        gravity = GetComponent<SimpleGravity>();
        ResetRandomQuackTimer();
    }

    void OnEnable()
    {
        gravity.OnBounce += HandleBounce;
    }

    void OnDisable()
    {
        gravity.OnBounce -= HandleBounce;
    }

    void Update()
    {
        HandleClickPause();
        HandleRandomQuack();
    }

    public void TriggerGroundClickQuack()
    {
        wander.SetWanderEnabled(false);
        wander.ResetToIdle();

        clickPauseTimer = clickQuackDuration;
        DuckPointsManager.Instance?.AddPoints(clickQuackPoints);
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

        if (!wander.enableWander || clickPauseTimer > 0f)
            return;

        randomQuackTimer -= Time.deltaTime;

        if (randomQuackTimer > 0f)
            return;

        TriggerRandomQuack();
        ResetRandomQuackTimer();
    }

    void TriggerRandomQuack()
    {
        DuckPointsManager.Instance?.AddPoints(randomQuackPoints);
    }

    void HandleBounce(SimpleGravity.BounceType bounceType, float impactSpeed)
    {
        if (!enableBouncePoints)
            return;

        if (bounceType == SimpleGravity.BounceType.SideWall ||
            bounceType == SimpleGravity.BounceType.Ceiling ||
            bounceType == SimpleGravity.BounceType.Ground)
        {
            DuckPointsManager.Instance?.AddPoints(randomQuackPoints);
        }
    }

    void ResetRandomQuackTimer()
    {
        randomQuackTimer = Random.Range(minRandomQuackDelay, maxRandomQuackDelay);
    }
}