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
    
    // Bat interaction settings
    [Header("Bat Interaction")]
    [SerializeField] private GameObject batObject;
    [SerializeField] private GameObject playerObject; // Added reference to the player object
    [SerializeField] private GameObject bossObject; // Reference to the boss object
    [SerializeField] private float normalBounceMultiplier = 2f; // For 1-2 unit range
    [SerializeField] private float closeRangeBounceMultiplier = 3.5f; // For 0-1 unit range
    [SerializeField] private float maxHitDistance = 4f; // Maximum distance for hitting the ball
    [SerializeField] private float perfectHitThreshold = 0.3f; // How close to max distance counts as perfect (in units)
    
    // Animation settings
    [Header("Animation")]
    [SerializeField] private string swingAnimationTrigger = "Swing"; // Name of the animation trigger parameter
    [SerializeField] private Animator _playerAnimator; // Reference to the player's animator component

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
    public GameObject BatObject => batObject;
    public GameObject PlayerObject => playerObject; // New property to access player object
    public GameObject BossObject => bossObject; // New property to access boss object
    public float NormalBounceMultiplier => normalBounceMultiplier;
    public float CloseRangeBounceMultiplier => closeRangeBounceMultiplier;
    public float CurveStrength => curveStrength;
    public float CurveResponse => curveResponse;
    public bool IsPerfectHit => _isPerfectHit;

    private void Awake()
    {
        // Add all required components
        _movementHandler = gameObject.AddComponent<BallMovementHandler>();
        _boostHandler = gameObject.AddComponent<BallBoostHandler>();
        _collisionHandler = gameObject.AddComponent<BallCollisionHandler>();
    }
    
    private void Start()
    {
        // Initialize handlers
        _movementHandler.Initialize(this);
        _boostHandler.Initialize(this, _movementHandler);
        _collisionHandler.Initialize(this, _movementHandler);
        
        // Get the player's animator component if player object exists
        if (playerObject != null)
        {
            if (_playerAnimator == null)
            {
                Debug.LogWarning("Player object does not have an Animator component");
            }
        }
    }
    
    private void Update()
    {
        // Check for bat hit input
        if (Input.GetKeyDown(KeyCode.E) && batObject != null && playerObject != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
            
            // Check if player can hit the ball
            if (distanceToPlayer <= maxHitDistance)
            {
                // Play bat swing animation
                if (_playerAnimator != null)
                {
                    _playerAnimator.SetTrigger(swingAnimationTrigger);
                }
                
                Vector3 directionToUse;
                float bounceMultiplier;
                
                // Check if this is a perfect hit (very close to max distance)
                if (distanceToPlayer >= maxHitDistance - perfectHitThreshold && bossObject != null)
                {
                    // Perfect hit! Direct towards the boss
                    directionToUse = (bossObject.transform.position - transform.position).normalized;
                    bounceMultiplier = normalBounceMultiplier * 1.5f; // Extra power for perfect hit
                    _isPerfectHit = true;
                    
                    // Visual/audio feedback for perfect hit could go here
                    Debug.Log("Perfect hit! Ball directed toward boss.");
                }
                else
                {
                    // Regular hit - use player's forward direction
                    directionToUse = playerObject.transform.forward;
                    bounceMultiplier = distanceToPlayer <= 2f ? closeRangeBounceMultiplier : normalBounceMultiplier;
                    _isPerfectHit = false;
                }
                
                _boostHandler.ApplyBatBoost(directionToUse, bounceMultiplier);
            }
        }
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
}