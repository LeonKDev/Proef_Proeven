using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    private BallController _controller;
    private BallMovementHandler _movementHandler;
    private GameObject _lastCollidedObject;

    private GameObject mainCamera;
    private HitStopEffect _hitStopEffect;
    private ScreenShake _screenShakeEffect;

    [SerializeField] private float _hitStopDuration = 0.03f;
    [SerializeField] private float _screenShakeStrength = 0.4f;

    private void Start()
    {
        mainCamera = GameObject.Find("CameraContainer");
        Debug.Log($"BallCollisionHandler: CameraContainer found: {mainCamera != null}");
        
        if (mainCamera != null)
        {
            _hitStopEffect = mainCamera.GetComponent<HitStopEffect>();
            _screenShakeEffect = mainCamera.GetComponent<ScreenShake>();
            Debug.Log($"BallCollisionHandler: HitStop component found: {_hitStopEffect != null}, ScreenShake component found: {_screenShakeEffect != null}");
        }
    }

    public void Initialize(BallController controller, BallMovementHandler movementHandler)
    {
        _controller = controller;
        _movementHandler = movementHandler;
        _lastCollidedObject = null;
        
        // Debug message to verify initialization
        Debug.Log("BallCollisionHandler initialized");
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if movement handler is initialized
        if (_movementHandler == null)
        {
            _movementHandler = GetComponent<BallMovementHandler>();
            if (_movementHandler == null)
            {
                Debug.LogError("BallCollisionHandler: MovementHandler not found");
                return;
            }
        }
        
        // Debug log for speed check
        Debug.Log($"Ball collision speed: {_movementHandler.CurrentSpeed}");
        
        // Apply visual effects if speed is high enough
        if (_movementHandler.CurrentSpeed >= 40)
        {
            Debug.Log("Speed threshold met for effects");
            if (_hitStopEffect != null && _screenShakeEffect != null)
            {
                Debug.Log("Triggering screen effects");
                StartCoroutine(_hitStopEffect.HitStopCoroutine(_hitStopDuration));
                _screenShakeEffect.StartScreenShake(_screenShakeStrength);
            }
            else
            {
                Debug.LogWarning($"Missing effects components - HitStop: {_hitStopEffect != null}, ScreenShake: {_screenShakeEffect != null}");
            }
        }

        // Reset perfect hit state whenever the ball collides with anything
        if (_controller != null)
        {
            _controller.ResetPerfectHitState();
        }

        if (collision.contacts.Length > 0)
        {
            // Store the last collided GameObject
            _lastCollidedObject = collision.gameObject;
            
            // Score handling
            if (_movementHandler.CurrentSpeed > 40f)
            {
                // Calculate point multiplier and add points accordingly
                float multiplier = _movementHandler.CurrentSpeed / 40;
                int pointsToAdd = (int)Mathf.Round(100 * multiplier);
                
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddPoints(pointsToAdd);
                    ScoreManager.Instance.InstantiateScoreObject(collision, pointsToAdd);
                }
            }
            
            // Debug messages for collision
            Debug.Log("Ball collision at speed: " + _movementHandler.CurrentSpeed);
            
            // Reflect the current direction based on collision normal
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(_movementHandler.Direction, normal);
            
            // Apply the new reflected direction
            _movementHandler.SetDirection(reflectedDirection);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Reset perfect hit state on trigger events as well
        if (_controller != null)
        {
            _controller.ResetPerfectHitState();
        }
        
        // Store last triggered GameObject
        _lastCollidedObject = other.gameObject;
    }
    
    /// <summary>
    /// Returns the last GameObject that this object collided or triggered with.
    /// </summary>
    /// <returns>The GameObject of the last collision or trigger, or null if none has occurred yet.</returns>
    public GameObject GetLastCollidedObject()
    {
        return _lastCollidedObject;
    }
}