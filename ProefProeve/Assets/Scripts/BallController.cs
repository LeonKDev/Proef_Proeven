using UnityEngine;
using BallGame.Shared;

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

    // Ball ownership
    [Header("Ball Ownership")]
    [SerializeField] private BallOwnerType ballOwner = BallOwnerType.Boss;

    // Curving behavior settings
    [Header("Curving Behavior")]
    [SerializeField] private float curveStrength = 0.02f; // How much to adjust the direction
    [SerializeField] private float curveResponse = 2f;    // How fast the adjustment is applied
    
    // References to other components
    private BallMovementHandler _movementHandler;
    private BallBoostHandler _boostHandler;
    private BallCollisionHandler _collisionHandler;
    private BallOwnerHandler _ownerHandler;

    // State tracking
    private bool _isPerfectHit = false;
    private bool _isHit = false;
    
    // Properties to access settings from other components
    public float BaseSpeed => baseSpeed;
    public float BoostMultiplier => boostMultiplier;
    public float BoostDuration => boostDuration;
    public float CurveStrength => curveStrength;
    public float CurveResponse => curveResponse;
    public bool IsPerfectHit => _isPerfectHit;
    public bool IsHit => _isHit;
    public GameObject PlayerObject => playerObject;
    public GameObject BossObject => bossObject;

    // Material ref
    public Material BossRef;
    public Material PlayerRef;

    // Property for ball owner
    public BallOwnerType BallOwner
    {
        get => ballOwner;
        set => ballOwner = value;
    }

    private void Awake()
    {
        // Add all required components
        _movementHandler = gameObject.AddComponent<BallMovementHandler>();
        _boostHandler = gameObject.AddComponent<BallBoostHandler>();
        _collisionHandler = gameObject.AddComponent<BallCollisionHandler>();
        _ownerHandler = gameObject.AddComponent<BallOwnerHandler>();

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
        _ownerHandler.Initialize(this);


        // Safely register this ball with the BallRegistrationManager
        SafeRegisterWithManager();
        
        // Disable the ball if game is not active
        CheckGameState();
    }
    
    private void Update()
    {
        switch (ballOwner)
        {
            case BallOwnerType.Player:
                // Do Player logic
                GetComponent<Renderer>().material = PlayerRef;
                break;
            case BallOwnerType.Boss:
                GetComponent<Renderer>().material = BossRef;
                // Do Boss logic
                break;
        }

        // Only process input and movement if game is active
        if (GameManager.Instance != null && !GameManager.Instance.isGameActive)
        {
            return;
        }
        
        // Normal update logic would go here if needed
    }
    
    /// <summary>
    /// Check game state and disable ball if game is not active
    /// </summary>
    private void CheckGameState()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameActive)
        {
            // Disable movement or other components that should only work when game is active
            if (_movementHandler != null)
            {
                _movementHandler.enabled = false;
            }
            
            if (_boostHandler != null)
            {
                _boostHandler.enabled = false;
            }
        }
    }
    
    /// <summary>
    /// Called by GameManager when game starts
    /// </summary>
    public void OnGameStart()
    {
        if (_movementHandler != null)
        {
            _movementHandler.enabled = true;
        }
        
        if (_boostHandler != null)
        {
            _boostHandler.enabled = true;
        }
    }
    
    /// <summary>
    /// Safely registers this ball with the BallRegistrationManager
    /// Creates the manager if it doesn't exist
    /// </summary>
    private void SafeRegisterWithManager()
    {
        // Safely register this ball with the BallRegistrationManager
        if (BallRegistrationManager.Instance != null)
        {
            BallRegistrationManager.Instance.RegisterBall(this);
        }
    }
    
    /// <summary>
    /// Sets the perfect hit state
    /// </summary>
    public void SetPerfectHit(bool isPerfect)
    {
        _isPerfectHit = isPerfect;
        _ownerHandler.SetOwner(BallOwnerType.Player);
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