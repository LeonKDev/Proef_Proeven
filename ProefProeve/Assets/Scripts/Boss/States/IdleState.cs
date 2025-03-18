using UnityEngine;

public class IdleState : State
{
    [Header("State Settings")]
    [SerializeField] private float currentAttackWaitTime;
    private float _oldAttackWaitTime;
    
    
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

    private void Start()
    {
        _oldAttackWaitTime = currentAttackWaitTime;
    }
    
    public override void Tick()
    {
        currentAttackWaitTime -= Time.deltaTime;

        if (currentAttackWaitTime <= 0.0f && _bossStats.HasBall)
        {
            currentAttackWaitTime = _oldAttackWaitTime;
            _stateMachine.ChangeState<AttackState>();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ball") && other.GetComponent<BallController>().IsPerfectHit)
        {
            Time.timeScale = 1;
            Destroy(other.gameObject);
            Time.timeScale = 1;
            
            _stateMachine.ChangeState<StaggeredState>();
        }
    }
}
