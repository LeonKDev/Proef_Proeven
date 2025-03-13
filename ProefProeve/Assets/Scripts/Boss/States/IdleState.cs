using System;
using UnityEngine;

public class IdleState : State
{
    protected StateMachine _stateMachine;
    
    [SerializeField] private float attackTime;
    [SerializeField] private Animator animator;
    
    private BossStats _bossStats;

    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
        _bossStats = GetComponent<BossStats>();
    }

    public override void Enter()
    {
        base.Enter();
        animator.SetTrigger("Idle");
        Debug.Log("enter idle");
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("exit Idle");
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
