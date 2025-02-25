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
        if (_controller.BatObject != null)
        {
            Vector3 toPlayer = (_controller.BatObject.transform.position - transform.position).normalized;
            
            // Reduce curve effect when currentSpeed is higher than baseSpeed
            float effectiveCurve = (_controller.CurveStrength * _controller.CurveResponse * Time.fixedDeltaTime) * 
                                  (_controller.BaseSpeed / _currentSpeed);
            
            Vector3 newDir = Vector3.Slerp(_rb.velocity.normalized, toPlayer, effectiveCurve).normalized;
            _rb.velocity = newDir * _currentSpeed;
            _direction = newDir;
        }
    }
}