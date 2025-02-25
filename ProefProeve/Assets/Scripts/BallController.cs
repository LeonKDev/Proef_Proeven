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
    [SerializeField] private float normalBounceMultiplier = 2f; // For 1-2 unit range
    [SerializeField] private float closeRangeBounceMultiplier = 3.5f; // For 0-1 unit range
    
    // Curving behavior settings
    [Header("Curving Behavior")]
    [SerializeField] private float curveStrength = 0.02f; // How much to adjust the direction
    [SerializeField] private float curveResponse = 2f;    // How fast the adjustment is applied
    
    // References to other components
    private BallMovementHandler _movementHandler;
    private BallBoostHandler _boostHandler;
    private BallCollisionHandler _collisionHandler;
    
    // Properties to access settings from other components
    public float BaseSpeed => baseSpeed;
    public float BoostMultiplier => boostMultiplier;
    public float BoostDuration => boostDuration;
    public GameObject BatObject => batObject;
    public float NormalBounceMultiplier => normalBounceMultiplier;
    public float CloseRangeBounceMultiplier => closeRangeBounceMultiplier;
    public float CurveStrength => curveStrength;
    public float CurveResponse => curveResponse;

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
    }
    
    private void Update()
    {
        // Check for bat hit input
        if (Input.GetKeyDown(KeyCode.E) && batObject != null)
        {
            float distanceToBat = Vector3.Distance(transform.position, batObject.transform.position);
            if (distanceToBat <= 4f)
            {
                Vector3 batDirection = batObject.transform.forward;
                float bounceMultiplier = distanceToBat <= 2f ? closeRangeBounceMultiplier : normalBounceMultiplier;
                _boostHandler.ApplyBatBoost(batDirection, bounceMultiplier);
            }
        }
        
        Debug.Log(Vector3.Distance(transform.position, batObject != null ? batObject.transform.position : Vector3.zero));
    }
}