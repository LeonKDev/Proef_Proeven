using UnityEngine;

public class StateMachine : MonoBehaviour
{
    // State data
    private State _currentState;
    private bool _inTransition;
    
    public State CurrentState => _currentState;
    
    private void Start()
    {
        // Sets the starting state of the boss
        ChangeState<IdleState>();
    }

    private void Update()
    {
        if (CurrentState != null && !_inTransition)
        {
            CurrentState.Tick();
        }
    }
    
    /// <summary>
    /// Changes the current state of the boss to the desired state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ChangeState<T>() where T : State
    {
        // Makes a reference to the state we want to change to
        T targetState = GetComponent<T>();

        if (targetState == null)
        {
            print("tried to change state to null state");
            return;
        }
        InitiateNewState(targetState);
    }

    private void InitiateNewState(State targetState)
    {
        if (_currentState != targetState && !_inTransition)
        {
            CallNewState(targetState);
        }
    }

    private void CallNewState(State newState)
    {
        _inTransition = true;
        
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();

        _inTransition = false;
    }
}

