using UnityEngine;

public class AttackState : State
{
    protected StateMachine _stateMachine;
    private BossStats _bossStats;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;

    private GameObject _currentBall;

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
        _currentBall = Instantiate(ballPrefab, ballSpawnPoint);
        _stateMachine.ChangeState<IdleState>();
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
