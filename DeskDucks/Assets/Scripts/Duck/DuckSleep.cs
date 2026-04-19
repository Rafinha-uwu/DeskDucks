using UnityEngine;

[RequireComponent(typeof(SimpleGravity))]
[RequireComponent(typeof(DuckWander))]
[RequireComponent(typeof(DuckStateController))]
public class DuckSleep : MonoBehaviour
{
    [Header("Sleep Duration")]
    [SerializeField] private float minSleepDuration = 4f;
    [SerializeField] private float maxSleepDuration = 10f;

    private SimpleGravity gravity;
    private DuckWander wander;
    private DuckStateController stateController;

    private Bed currentBed;
    private float sleepTimer;
    private bool isSleeping;

    public bool IsSleeping => isSleeping;

    void Awake()
    {
        gravity = GetComponent<SimpleGravity>();
        wander = GetComponent<DuckWander>();
        stateController = GetComponent<DuckStateController>();
    }

    void Update()
    {
        if (!isSleeping)
            return;

        if (currentBed == null)
        {
            WakeUp();
            return;
        }

        transform.position = currentBed.GetDuckSleepWorldPosition();

        sleepTimer -= Time.deltaTime;
        if (sleepTimer <= 0f)
            WakeUp();
    }

    public bool TryStartSleeping(Bed bed)
    {
        if (bed == null)
            return false;

        if (isSleeping)
            return false;

        currentBed = bed;
        isSleeping = true;
        sleepTimer = Random.Range(minSleepDuration, maxSleepDuration);

        if (wander != null)
            wander.SetWanderEnabled(false);

        if (gravity != null)
        {
            gravity.ResetVelocity();
            gravity.SetGravityEnabled(false);
        }

        transform.position = currentBed.GetDuckSleepWorldPosition();
        stateController?.SetStateImmediate(DuckStateController.DuckState.Sleeping);

        return true;
    }

    public void CancelSleepForExternalControl()
    {
        if (!isSleeping)
            return;

        isSleeping = false;
        currentBed = null;

        if (gravity != null)
        {
            gravity.SetGravityEnabled(true);
            gravity.ResetVelocity();
        }

        if (wander != null)
            wander.SetWanderEnabled(false);
    }

    public void WakeUp()
    {
        if (!isSleeping)
            return;

        isSleeping = false;

        Vector2 wakePosition = transform.position;

        if (currentBed != null)
            wakePosition = currentBed.GetDuckWakeWorldPosition();

        transform.position = new Vector3(wakePosition.x, wakePosition.y, transform.position.z);

        if (gravity != null)
        {
            gravity.SetGravityEnabled(true);
            gravity.ResetVelocity();
        }

        if (wander != null)
        {
            wander.ResetToIdle();
            wander.SetWanderEnabled(true);
        }

        stateController?.SetStateImmediate(DuckStateController.DuckState.Idle);
        currentBed = null;
    }
}