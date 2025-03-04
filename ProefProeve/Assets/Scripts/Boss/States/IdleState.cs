using UnityEngine;

public class IdleState : State
{
    protected StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("enter IdleState");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit IdleState");
    }
    
    public override void Tick()
    {
        base.Tick();
        if (Input.GetKey(KeyCode.Space))
        {
            _stateMachine.ChangeState<AttackState>();
        }
    }
}
