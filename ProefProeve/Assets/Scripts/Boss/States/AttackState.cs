using UnityEngine;

public class AttackState : State
{
    protected StateMachine _stateMachine;
    private BossStats _bossStats;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    
    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
        _bossStats = GetComponent<BossStats>();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter AttackState");
        
        //spawns the ball
        Instantiate(ballPrefab, ballSpawnPoint);
        _bossStats.HasBall = false;
        _stateMachine.ChangeState<IdleState>();
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("Exit AttackState");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            _stateMachine.ChangeState<StaggeredState>();
        }
    }
}
