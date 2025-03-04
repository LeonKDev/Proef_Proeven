using UnityEngine;

public class AttackState : State
{
    protected StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter AttackState");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit AttackState");
    }
    
    public override void Tick()
    {
        base.Tick();
    }
}
