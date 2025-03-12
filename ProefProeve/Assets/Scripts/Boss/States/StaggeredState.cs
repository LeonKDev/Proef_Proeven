using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggeredState : State
{
    [SerializeField] private float staggerTime;
    protected StateMachine _stateMachine;
    private BossStats _bossStats;
    public GameObject test;
    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
        _bossStats = GetComponent<BossStats>();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter StaggeredState");
        test.GetComponent<Renderer>().material.color = Color.yellow;
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit StaggeredState");
    }
    
    public override void Tick()
    {
        base.Tick();
        staggerTime -= Time.deltaTime;

        if (staggerTime <= 0.0f)
        {
            _stateMachine.ChangeState<IdleState>();
        }
    }
}
