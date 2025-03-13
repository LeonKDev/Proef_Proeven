using UnityEngine;

public class BallBoostHandler : MonoBehaviour
{
    private BallController _controller;
    private BallMovementHandler _movementHandler;
    
    private float _boostTimeRemaining;
    private float _initialBoostSpeed;
    private bool _initialized = false;

    public void Initialize(BallController controller, BallMovementHandler movementHandler)
    {
        _controller = controller;
        _movementHandler = movementHandler;
        _initialized = true;
    }

    void FixedUpdate()
    {
        if (_initialized)
        {
            HandleBoostDecay();
        }
    }
    
    public void ApplyBatBoost(Vector3 direction, float bounceMultiplier)
    {
        if (!_initialized || _controller == null || _movementHandler == null)
        {
            Debug.LogError("BallBoostHandler: Cannot apply boost - not properly initialized");
            return;
        }
        
        // Apply boost effect on hit
        float boostedSpeed = _controller.BaseSpeed * _controller.BoostMultiplier * bounceMultiplier;
        _initialBoostSpeed = boostedSpeed;
        _movementHandler.CurrentSpeed = boostedSpeed;
        _boostTimeRemaining = _controller.BoostDuration;
        
        // Set the new direction
        _movementHandler.SetDirection(direction.normalized);
    }
    
    private void HandleBoostDecay()
    {
        if (!_initialized || _controller == null || _movementHandler == null)
        {
            return;
        }
        
        if (_boostTimeRemaining > 0)
        {
            _boostTimeRemaining -= Time.fixedDeltaTime;
            float progress = 1 - Mathf.Clamp01(_boostTimeRemaining / _controller.BoostDuration);
            _movementHandler.CurrentSpeed = Mathf.Lerp(_initialBoostSpeed, _controller.BaseSpeed, progress);
        }
        else
        {
            _movementHandler.CurrentSpeed = _controller.BaseSpeed;
        }
    }
}