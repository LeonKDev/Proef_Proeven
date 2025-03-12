using UnityEngine;

public class BallController : MonoBehaviour
{
    // Movement settings
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 5f;
    
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
    
    // Properties to access settings from other components
    public float BaseSpeed => baseSpeed;
    public float BoostMultiplier => boostMultiplier;
    public float BoostDuration => boostDuration;
    public float CurveStrength => curveStrength;
    public float CurveResponse => curveResponse;
    public bool IsPerfectHit => _isPerfectHit;
    public GameObject PlayerObject => playerObject;
    public GameObject BossObject => bossObject;

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
    
    /// <summary>
    /// Sets the perfect hit state
    /// </summary>
    public void SetPerfectHit(bool isPerfect)
    {
        _isPerfectHit = isPerfect;
    }
    
    /// <summary>
    /// Returns the last GameObject that the ball collided or triggered with.
    /// </summary>
    /// <returns>The GameObject from the last collision or trigger event, or null if none has occurred.</returns>
    public GameObject GetLastCollidedObject()
    {
        return _collisionHandler.GetLastCollidedObject();
    }
    
    /// <summary>
    /// Resets the perfect hit state, typically called when the ball hits something
    /// </summary>
    public void ResetPerfectHitState()
    {
        _isPerfectHit = false;
    }
    
    /// <summary>
    /// Gets the boost handler component
    /// </summary>
    public BallBoostHandler GetBoostHandler()
    {
        return _boostHandler;
    }
    
    /// <summary>
    /// Gets the movement handler component
    /// </summary>
    public BallMovementHandler GetMovementHandler()
    {
        return _movementHandler;
    }
}