using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallMovementHandler : MonoBehaviour
{
    private BallController _controller;
    private Rigidbody _rb;
    private Vector3 _direction;
    private float _currentSpeed;

    public Vector3 Direction => _direction;
    public float CurrentSpeed 
    {
        get => _currentSpeed; 
        set => _currentSpeed = value;
    }

    public void Initialize(BallController controller)
    {
        _controller = controller;
        _rb = GetComponent<Rigidbody>();
        _currentSpeed = _controller.BaseSpeed;

        // Use the initial direction from BallController
        _direction = _controller.InitialVelocityDirection;
        
        // Apply initial movement
        ApplyVelocity();
        
        // Debug message to verify initialization
        Debug.Log($"BallMovementHandler initialized with base speed: {_currentSpeed} and direction: {_direction}");
    }

    void FixedUpdate()
    {
        // Keep movement in the horizontal plane
        if (_rb != null && _rb.velocity != Vector3.zero)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

            // Update direction if velocity has changed
            if (_rb.velocity.magnitude != 0)
            {
                _direction = _rb.velocity.normalized;
            }

            // Apply velocity with current direction and speed
            ApplyVelocity();

            // Apply curving effect
            ApplyCurving();
        }
    }

    public void SetDirection(Vector3 newDirection)
    {
        _direction = new Vector3(newDirection.x, 0f, newDirection.z).normalized;
        ApplyVelocity();
        Debug.Log($"Setting ball direction to: {_direction} with speed: {_currentSpeed}");
    }

    public void ApplyVelocity()
    {
        if (_rb != null)
        {
            _rb.velocity = _direction * _currentSpeed;
        }
    }

    private void ApplyCurving()
    {
        if (_controller == null) return;
        
        // Determine which object to curve towards based on perfect hit state
        GameObject targetObject = null;
        
        if (_controller.IsPerfectHit && _controller.BossObject != null)
        {
            // If perfect hit, curve toward the boss
            targetObject = _controller.BossObject;
        }
        else if (_controller.PlayerObject != null)
        {
            // Otherwise use the player object for curving
            targetObject = _controller.PlayerObject;
        }
        
        if (targetObject != null)
        {
            Vector3 toTarget = (targetObject.transform.position - transform.position).normalized;
            
            // Reduce curve effect when currentSpeed is higher than baseSpeed
            float effectiveCurve = (_controller.CurveStrength * _controller.CurveResponse * Time.fixedDeltaTime);
            
            // Scale curve effect based on speed ratio
            if (_currentSpeed > 0)
            {
                effectiveCurve *= (_controller.BaseSpeed / _currentSpeed);
            }
            
            // If it's a perfect hit, increase the curve strength to ensure it targets the boss more aggressively
            if (_controller.IsPerfectHit && targetObject == _controller.BossObject)
            {
                effectiveCurve *= 2f; // Double the curve effect for perfect hits
            }
            
            // Prevent NaN when velocity is zero
            if (_rb.velocity.magnitude > 0.01f)
            {
                Vector3 newDir = Vector3.Slerp(_rb.velocity.normalized, toTarget, effectiveCurve).normalized;
                _rb.velocity = newDir * _currentSpeed;
                _direction = newDir;
            }
        }
    }
}