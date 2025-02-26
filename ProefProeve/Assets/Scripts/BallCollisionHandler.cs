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

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        _hitStopEffect = mainCamera.GetComponent<HitStopEffect>();
        _screenShakeEffect = mainCamera.GetComponent<ScreenShake>();

        _ballMovement = this.gameObject.GetComponent<BallMovementHandler>();
    }

    private void Update()
    {
        Debug.Log(_ballMovement.CurrentSpeed);
    }

    public void Initialize(BallController controller, BallMovementHandler movementHandler)
    {
        _controller = controller;
        _movementHandler = movementHandler;
        _lastCollidedObject = null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(_ballMovement.CurrentSpeed >= 40)
        {
            StartCoroutine(_hitStopEffect.HitStopCoroutine(0.03f));
            _screenShakeEffect.StartScreenShake(0.4f);
        }

        if (collision.contacts.Length > 0)
        {
            // Store the last collided GameObject
            _lastCollidedObject = collision.gameObject;
            
            // Reflect the current direction based on collision normal
            Vector3 normal = collision.contacts[0].normal;
            Vector3 reflectedDirection = Vector3.Reflect(_movementHandler.Direction, normal);
            
            // Apply the new reflected direction
            _movementHandler.SetDirection(reflectedDirection);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
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