using UnityEngine;

public class AttackState : State
{
    protected StateMachine StateMachine;
    private BossStats _bossStats;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    public GameObject test;
    private void Awake()
    {
        StateMachine = GetComponent<StateMachine>();
        _bossStats = GetComponent<BossStats>();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter AttackState");
        test.GetComponent<Renderer>().material.color = Color.red;
        
        //spawns the ball
        Instantiate(ballPrefab, ballSpawnPoint);
        _bossStats.HasBall = false;
        StateMachine.ChangeState<IdleState>();
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
            StateMachine.ChangeState<StaggeredState>();
        }
    }
}
