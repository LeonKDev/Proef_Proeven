using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggeredState : State
{
    protected StateMachine _stateMachine;
    
    [SerializeField] private float staggerTime;
    [SerializeField] private Animator animator;
    
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
        animator.SetTrigger("Staggerd");
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
