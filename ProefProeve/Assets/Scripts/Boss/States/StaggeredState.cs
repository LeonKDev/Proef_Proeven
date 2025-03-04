using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggeredState : State
{
    protected StateMachine _stateMachine;
    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter StaggeredState");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit StaggeredState");
    }
    
    public override void Tick()
    {
        base.Tick();
    }
}
