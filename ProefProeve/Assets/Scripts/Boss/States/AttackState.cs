using UnityEngine;

public class AttackState : State
{
    protected StateMachine _stateMachine;
    
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
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
        Debug.Log("enter attack");
        if (!_bossStats.HasBall)
        {
            _stateMachine.ChangeState<IdleState>();
        }
        
        animator.SetTrigger("Attack");
        
        //spawns the ball
        Instantiate(ballPrefab, ballSpawnPoint);
        _bossStats.HasBall = false;
        
        _stateMachine.ChangeState<IdleState>();
    }
    
    public override void Exit()
    {
        base.Exit();
        Debug.Log("exit attack");
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _stateMachine.ChangeState<StaggeredState>();
        }
    }
}
