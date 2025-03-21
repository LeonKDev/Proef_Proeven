using UnityEngine;

public class AttackState : State
{
    [Header("Ball References")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject ballSpawnPoint;
    
    // References to other components
    private StateMachine _stateMachine;
    private BossStats _bossStats;
    private Animator _animator;
    
    private void Awake()
    {
        _stateMachine = GetComponent<StateMachine>();
        _bossStats = GetComponent<BossStats>();
        _animator = GetComponentInChildren<Animator>();
    }
    
    public override void Enter()
    {
        if (!_bossStats.HasBall)
        {
            _stateMachine.ChangeState<IdleState>();
        }
        
        _animator.SetTrigger("Attack");
        
        //spawns the ball
        Instantiate(ballPrefab, ballSpawnPoint.transform.position ,Quaternion.Euler(Vector3.forward));
        
    }

    public override void Tick()
    {
        base.Tick();
        if (_bossStats.HasBall == false)
        {
            _stateMachine.ChangeState<IdleState>();
        }
    }
}

