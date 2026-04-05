using UnityEngine;

[RequireComponent(typeof(DuckWander))]
[RequireComponent(typeof(SimpleGravity))]
public class DuckQuack : MonoBehaviour
{
    [Header("Click Quack")]
    public float clickQuackDuration = 1f;
    public int clickQuackPoints = 1;

    [Header("Random Quack")]
    public bool enableRandomQuacks = true;
    public float minRandomQuackDelay = 5f;
    public float maxRandomQuackDelay = 20f;
    public int randomQuackPoints = 1;

    [Header("Bounce Points")]
    public bool enableBouncePoints = true;

    private DuckWander wander;
    private SimpleGravity gravity;

    private float clickPauseTimer = 0f;
    private float randomQuackTimer = 0f;

    public bool IsBusy => clickPauseTimer > 0f;
    public bool LastClickWasAir { get; private set; }

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
        LastClickWasAir = false;

        wander.enableWander = false;
        wander.ForceIdle();

        clickPauseTimer = clickQuackDuration;

        DuckPointsManager.Instance?.AddPoints(clickQuackPoints);

        // FUTURE: play grounded click quack animation here
        // FUTURE: play grounded click quack sound here
    }

    public void TriggerAirClickQuack()
    {
        LastClickWasAir = true;

        wander.enableWander = false;

        clickPauseTimer = clickQuackDuration;

        DuckPointsManager.Instance?.AddPoints(clickQuackPoints);

        // FUTURE: play air click quack animation here
        // FUTURE: play air click quack sound here
    }

    void HandleClickPause()
    {
        if (clickPauseTimer <= 0f)
            return;

        clickPauseTimer -= Time.deltaTime;

        if (clickPauseTimer > 0f)
            return;

        clickPauseTimer = 0f;

        wander.ForceIdle();
        wander.enableWander = true;

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

        // FUTURE: temporary random quack animation here
        // FUTURE: play random quack sound here
    }

    void HandleBounce(SimpleGravity.BounceType bounceType, float impactSpeed)
    {
        if (!enableBouncePoints)
            return;

        if (bounceType == SimpleGravity.BounceType.SideWall ||
            bounceType == SimpleGravity.BounceType.Ground ||
            bounceType == SimpleGravity.BounceType.Ceiling)
        {
            DuckPointsManager.Instance?.AddPoints(randomQuackPoints);

            // FUTURE: bounce sound / effect here
        }
    }

    void ResetRandomQuackTimer()
    {
        randomQuackTimer = Random.Range(minRandomQuackDelay, maxRandomQuackDelay);
    }
}