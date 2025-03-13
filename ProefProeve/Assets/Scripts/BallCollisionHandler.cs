using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    private BallController _controller;
    private BallMovementHandler _movementHandler;
    private GameObject _lastCollidedObject;

    private GameObject mainCamera;
    private HitStopEffect _hitStopEffect;
    private ScreenShake _screenShakeEffect;
    private SparkEffectHandler _sparkEffectHandler;

    [SerializeField] private float _hitStopDuration = 0.03f;
    [SerializeField] private float _screenShakeStrength = 0.4f;
    [SerializeField] private float _sparkEffectSpeedThreshold = 35f; // New speed threshold for spark effects

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

        _sparkEffectHandler = GetComponent<SparkEffectHandler>();
        if (_sparkEffectHandler == null)
        {
            _sparkEffectHandler = gameObject.AddComponent<SparkEffectHandler>();
        }
    }

    public void Initialize(BallController controller, BallMovementHandler movementHandler)
    {
        _controller = controller;
        _movementHandler = movementHandler;
        _lastCollidedObject = null;
        
        Debug.Log("BallCollisionHandler initialized");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_movementHandler == null)
        {
            _movementHandler = GetComponent<BallMovementHandler>();
            if (_movementHandler == null)
            {
                Debug.LogError("BallCollisionHandler: MovementHandler not found");
                return;
            }
        }
        
        Debug.Log($"Ball collision speed: {_movementHandler.CurrentSpeed}");
        
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

        if (_controller != null)
        {
            _controller.ResetPerfectHitState();
        }

        if (collision.contacts.Length > 0)
        {
            _lastCollidedObject = collision.gameObject;
            
            if (_movementHandler.CurrentSpeed > 40f)
            {
                float multiplier = _movementHandler.CurrentSpeed / 40;
                int pointsToAdd = (int)Mathf.Round(100 * multiplier);
                
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddPoints(pointsToAdd);
                    ScoreManager.Instance.InstantiateScoreObject(collision, pointsToAdd);
                }
            }
            
            Debug.Log("Ball collision at speed: " + _movementHandler.CurrentSpeed);
            
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(_movementHandler.Direction, normal);
            
            _movementHandler.SetDirection(reflectedDirection);

            // Spawn spark effect at collision point if speed threshold is met
            if (_sparkEffectHandler != null && _movementHandler.CurrentSpeed >= _sparkEffectSpeedThreshold)
            {
                _sparkEffectHandler.SpawnSpark(collision.contacts[0].point, normal);
            }
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        _lastCollidedObject = other.gameObject;
    }
    
    public GameObject GetLastCollidedObject()
    {
        return _lastCollidedObject;
    }
}