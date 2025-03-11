using System;
using UnityEngine;

public class IdleState : State
{
    protected StateMachine _stateMachine;
    private BossStats _bossStats;
    [SerializeField] private float attackTime;

    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
        _bossStats = GetComponent<BossStats>();
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
        attackTime -= Time.deltaTime;

        if (attackTime <= 0.0f && _bossStats.HasBall)
        {
            _stateMachine.ChangeState<AttackState>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _stateMachine.ChangeState<StaggeredState>();
        }
    }
}
