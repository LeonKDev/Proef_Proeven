using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public State CurrentState => _currentState;
    private State _currentState;
    
    protected bool InTransition;

    private void Start()
    {
        ChangeState<IdleState>();
    }

    private void Update()
    {
        if (CurrentState != null && !InTransition)
        {
            CurrentState.Tick();
        }
    }

    public void ChangeState<T>() where T : State
    {
        T targetState = GetComponent<T>();

        if (targetState == null)
        {
            print("tried to change state to null state");
            return;
        }
        InitiateNewState(targetState);
    }

    public void InitiateNewState(State targetState)
    {
        if (_currentState != targetState && !InTransition)
        {
            CallNewState(targetState);
        }
    }

    public void CallNewState(State newState)
    {
        InTransition = true;
        
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();

        InTransition = false;
    }
}

