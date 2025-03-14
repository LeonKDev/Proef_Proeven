using UnityEngine;
using BallGame.Shared;

public class BallController : MonoBehaviour
{
    // Movement settings
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private Vector3 initialVelocityDirection = Vector3.right; // Default direction
    [SerializeField] private bool useRandomInitialDirection = true; // Option to use random or specified direction
    
    // Boost settings
    [Header("Boost Settings")]
    [SerializeField] private float boostMultiplier = 3f;
    [SerializeField] private float boostDuration = 0.5f;
    
    // References for targeting
    [Header("Targeting References")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject bossObject;
    
    // Curving behavior settings
    [Header("Curving Behavior")]
    [SerializeField] private float curveStrength = 0.02f; // How much to adjust the direction
    [SerializeField] private float curveResponse = 2f;    // How fast the adjustment is applied
    
    // References to other components
    private BallMovementHandler _movementHandler;
    private BallBoostHandler _boostHandler;
    private BallCollisionHandler _collisionHandler;
    
    // State tracking
    private bool _isPerfectHit = false;
    private BallOwnerType _currentOwner = BallOwnerType.Boss;
    
    // Properties to access settings from other components
    public float BaseSpeed => baseSpeed;
    public float BoostMultiplier => boostMultiplier;
    public float BoostDuration => boostDuration;
    public float CurveStrength => curveStrength;
    public float CurveResponse => curveResponse;
    public bool IsPerfectHit => _isPerfectHit;
    public GameObject PlayerObject => playerObject;
    public GameObject BossObject => bossObject;
    public Vector3 InitialVelocityDirection => useRandomInitialDirection ? 
        new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized : 
        initialVelocityDirection.normalized;

    // Material ref
    public Material BossRef;
    public Material PlayerRef;
    public GameObject trailRenderer;

    // Ball ownership property
    public BallOwnerType BallOwner
    {
        get => _currentOwner;
        set => _currentOwner = value;
    }

    private void Update()
    {
        switch (_currentOwner)
        {
            case BallOwnerType.Player:
                // Do Player logic
                GetComponent<Renderer>().material = PlayerRef;
                trailRenderer.GetComponent<Renderer>().material = PlayerRef;
                break;
            case BallOwnerType.Boss:
                GetComponent<Renderer>().material = BossRef;
                trailRenderer.GetComponent<Renderer>().material = BossRef;
                // Do Boss logic
                break;
        }
    }

    private void Awake()
    {
        // Add all required components
        _movementHandler = gameObject.AddComponent<BallMovementHandler>();
        _boostHandler = gameObject.AddComponent<BallBoostHandler>();
        _collisionHandler = gameObject.AddComponent<BallCollisionHandler>();
        
        // Find reference objects if not set
        if (playerObject == null) {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (bossObject == null) {
            bossObject = GameObject.FindGameObjectWithTag("Boss");
        }
    }
    
    private void Start()
    {
        // Initialize handlers
        _movementHandler.Initialize(this);
        _boostHandler.Initialize(this, _movementHandler);
        _collisionHandler.Initialize(this, _movementHandler);
        
        // Register this ball with the BallRegistrationManager
        BallRegistrationManager.Instance.RegisterBall(this);
    }
    
    private void OnDestroy()
    {
        // Unregister this ball when destroyed
        if (BallRegistrationManager.Instance != null)
        {
            BallRegistrationManager.Instance.UnregisterBall(this);
        }
    }
    
    public void SetPerfectHit(bool isPerfect)
    {
        _isPerfectHit = isPerfect;
    }
    
    public GameObject GetLastCollidedObject()
    {
        return _collisionHandler.GetLastCollidedObject();
    }
    
    public void ResetPerfectHitState()
    {
        _isPerfectHit = false;
    }
    
    public BallBoostHandler GetBoostHandler()
    {
        return _boostHandler;
    }
    
    public BallMovementHandler GetMovementHandler()
    {
        return _movementHandler;
    }
}