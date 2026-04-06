using UnityEngine;

[RequireComponent(typeof(DuckStateController))]
public class DuckAnimationController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    private DuckStateController stateController;

    void Awake()
    {
        stateController = GetComponent<DuckStateController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        stateController.OnStateChanged += HandleStateChanged;
    }

    void OnDisable()
    {
        stateController.OnStateChanged -= HandleStateChanged;
    }

    void Start()
    {
        ApplyState(stateController.CurrentState);
    }

    void HandleStateChanged(DuckStateController.DuckState previousState, DuckStateController.DuckState newState)
    {
        ApplyState(newState);
    }

    void ApplyState(DuckStateController.DuckState state)
    {
        if (animator == null)
            return;

        switch (state)
        {
            case DuckStateController.DuckState.Idle:
                // Later: play/set your idle animation here.
                // Example later:
                // animator.Play("Idle");
                break;

            case DuckStateController.DuckState.Walking:
                // Later: play/set your walking animation here.
                // Example later:
                // animator.Play("Walk");
                break;

            case DuckStateController.DuckState.ClickQuack:
                // Later: play/set your click quack animation here.
                // Example later:
                // animator.Play("ClickQuack");
                break;

            case DuckStateController.DuckState.Dragged:
                // Later: play/set your dragged/held pose here.
                // Example later:
                // animator.Play("Dragged");
                break;

            case DuckStateController.DuckState.Airborne:
                // Later: play/set your airborne animation here.
                // Example later:
                // animator.Play("Airborne");
                break;

            case DuckStateController.DuckState.LandingRecovery:
                // Later: play/set your landing animation here.
                // Example later:
                // animator.Play("Land");
                break;
        }
    }
}