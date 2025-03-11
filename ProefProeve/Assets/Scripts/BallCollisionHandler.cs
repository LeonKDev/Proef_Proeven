using UnityEngine;

public class BallCollisionHandler : MonoBehaviour
{
    private BallController _controller;
    private BallMovementHandler _movementHandler;
    private GameObject _lastCollidedObject;

    private GameObject mainCamera;
    private HitStopEffect _hitStopEffect;
    private ScreenShake _screenShakeEffect;
    public BallMovementHandler _ballMovement;

    [SerializeField] private float _hitStopDuration = 0.03f;
    [SerializeField] private float _screenShakeStrength = 0.4f;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        _hitStopEffect = mainCamera.GetComponent<HitStopEffect>();
        _screenShakeEffect = mainCamera.GetComponent<ScreenShake>();

        _ballMovement = this.gameObject.GetComponent<BallMovementHandler>();
    }

    public void Initialize(BallController controller, BallMovementHandler movementHandler)
    {
        _controller = controller;
        _movementHandler = movementHandler;
        _lastCollidedObject = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_ballMovement.CurrentSpeed >= 40)
        {
            StartCoroutine(_hitStopEffect.HitStopCoroutine(_hitStopDuration));
            _screenShakeEffect.StartScreenShake(_screenShakeStrength);
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
            
            if (_movementHandler.CurrentSpeed > 40f)
            {
                // calculate point multiplier and add points accordingly
                float multiplier = _movementHandler.CurrentSpeed / 40;
                int pointsToAdd = (int)Mathf.Round(100 * multiplier);
                ScoreManager.Instance.AddPoints(pointsToAdd);
                
                // instantiates the score object at the current collision point
                ScoreManager.Instance.InstantiateScoreObject(collision, pointsToAdd);
            }
            
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