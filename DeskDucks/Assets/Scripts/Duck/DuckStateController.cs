using System;
using UnityEngine;

public class DuckStateController : MonoBehaviour
{
    public enum DuckState
    {
        Idle,
        Walking,
        ClickQuack,
        Dragged,
        Airborne,
        LandingRecovery,
        Sleeping
    }

    public DuckState CurrentState { get; private set; } = DuckState.Idle;

    public event Action<DuckState, DuckState> OnStateChanged;

    public bool IsAutonomousState =>
        CurrentState == DuckState.Idle ||
        CurrentState == DuckState.Walking;

    public bool IsBusyState =>
        CurrentState == DuckState.ClickQuack ||
        CurrentState == DuckState.Dragged ||
        CurrentState == DuckState.LandingRecovery ||
        CurrentState == DuckState.Sleeping;

    public bool CanStartDrag =>
        CurrentState != DuckState.Dragged &&
        CurrentState != DuckState.Sleeping;

    public bool CanStartClickQuack =>
        CurrentState != DuckState.Dragged &&
        CurrentState != DuckState.Sleeping;

    public bool SetState(DuckState newState)
    {
        if (CurrentState == newState)
            return false;

        if (!IsTransitionAllowed(CurrentState, newState))
            return false;

        DuckState previous = CurrentState;
        CurrentState = newState;
        OnStateChanged?.Invoke(previous, newState);
        return true;
    }

    public void SetStateImmediate(DuckState newState)
    {
        if (CurrentState == newState)
            return;

        DuckState previous = CurrentState;
        CurrentState = newState;
        OnStateChanged?.Invoke(previous, newState);
    }

    private bool IsTransitionAllowed(DuckState from, DuckState to)
    {
        switch (from)
        {
            case DuckState.Dragged:
                return to == DuckState.Airborne ||
                       to == DuckState.Idle;

            case DuckState.ClickQuack:
                return to == DuckState.Idle ||
                       to == DuckState.Airborne ||
                       to == DuckState.LandingRecovery ||
                       to == DuckState.Sleeping;

            case DuckState.Airborne:
                return to == DuckState.Idle ||
                       to == DuckState.Walking ||
                       to == DuckState.ClickQuack ||
                       to == DuckState.LandingRecovery ||
                       to == DuckState.Dragged ||
                       to == DuckState.Sleeping;

            case DuckState.LandingRecovery:
                return to == DuckState.Idle ||
                       to == DuckState.Walking ||
                       to == DuckState.ClickQuack ||
                       to == DuckState.Dragged ||
                       to == DuckState.Sleeping;

            case DuckState.Sleeping:
                return to == DuckState.Idle ||
                       to == DuckState.Walking ||
                       to == DuckState.Dragged;

            case DuckState.Idle:
            case DuckState.Walking:
                return true;

            default:
                return true;
        }
    }
}