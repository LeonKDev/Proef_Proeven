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

        // Set initial random direction
        _direction = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;
        
        // Apply initial movement
        ApplyVelocity();
    }

    void FixedUpdate()
    {
        // Keep movement in the horizontal plane
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

    public void SetDirection(Vector3 newDirection)
    {
        _direction = new Vector3(newDirection.x, 0f, newDirection.z).normalized;
        ApplyVelocity();
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
        // Determine which object to curve towards based on perfect hit state
        GameObject targetObject;
        
        if (_controller.IsPerfectHit && _controller.BossObject != null)
        {
            // If perfect hit, curve toward the boss
            targetObject = _controller.BossObject;
        }
        else
        {
            // Otherwise use the player object for curving if available, or fall back to the bat
            targetObject = _controller.PlayerObject != null ? _controller.PlayerObject : _controller.BatObject;
        }
        
        if (targetObject != null)
        {
            Vector3 toTarget = (targetObject.transform.position - transform.position).normalized;
            
            // Reduce curve effect when currentSpeed is higher than baseSpeed
            float effectiveCurve = (_controller.CurveStrength * _controller.CurveResponse * Time.fixedDeltaTime) * 
                                  (_controller.BaseSpeed / _currentSpeed);
            
            // If it's a perfect hit, increase the curve strength to ensure it targets the boss more aggressively
            if (_controller.IsPerfectHit && targetObject == _controller.BossObject)
            {
                effectiveCurve *= 2f; // Double the curve effect for perfect hits
            }
            
            Vector3 newDir = Vector3.Slerp(_rb.velocity.normalized, toTarget, effectiveCurve).normalized;
            _rb.velocity = newDir * _currentSpeed;
            _direction = newDir;
        }
    }
}