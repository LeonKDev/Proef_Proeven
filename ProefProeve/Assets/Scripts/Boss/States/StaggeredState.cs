using UnityEngine;

public class StaggeredState : State
{
    [Header("State Settings")]
    [SerializeField] private float staggerTime;
    private float currentStaggerTime;

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
        currentStaggerTime = staggerTime;
    }

    public override void Enter()
    {
        _animator.SetTrigger("Staggerd");
        _bossStats.DamageBoss(1);
    }
    
    public override void Tick()
    {
        currentStaggerTime -= Time.deltaTime;

        if (currentStaggerTime <= 0.0f)
        {
            currentStaggerTime = staggerTime;
            _bossStats.HasBall = true;
            _stateMachine.ChangeState<IdleState>();
        }
    }
}
