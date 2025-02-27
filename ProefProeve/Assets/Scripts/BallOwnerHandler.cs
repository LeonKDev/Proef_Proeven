using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallGame.Shared;

/// <summary>
/// Handles ball ownership logic and transitions
/// </summary>
public class BallOwnerHandler : MonoBehaviour
{
    [SerializeField] private float ownershipTimeoutDuration = 3f; // Time after which ball ownership changes to Boss
    
    private BallController _ballController;
    private float _lastPlayerInteractionTime;
    private BallOwnerType _currentOwner = BallOwnerType.Boss;
    
    public void Initialize(BallController controller)
    {
        _ballController = controller;
        _lastPlayerInteractionTime = Time.time;
        _currentOwner = BallOwnerType.Boss;
    }
    
    private void Update()
    {
        // Check if player owns the ball
        if (_currentOwner == BallOwnerType.Player)
        {
            // Check if the ownership timeout has passed
            if (Time.time - _lastPlayerInteractionTime >= ownershipTimeoutDuration)
            {
                // Transfer ownership to Boss
                SetOwner(BallOwnerType.Boss);
            }
        }
        
        // Ensure the BallController's ownership value is kept in sync
        if (_ballController != null)
        {
            _ballController.BallOwner = _currentOwner;
        }
    }
    
    /// <summary>
    /// Sets the current ball owner and updates the last interaction time if the player is the owner
    /// </summary>
    public void SetOwner(BallOwnerType owner)
    {
        _currentOwner = owner;
        
        // Reset the timer when player gets ownership
        if (owner == BallOwnerType.Player)
        {
            _lastPlayerInteractionTime = Time.time;
        }
        
        // Ensure the BallController's ownership value is updated
        if (_ballController != null)
        {
            _ballController.BallOwner = _currentOwner;
        }
    }
    
    /// <summary>
    /// Returns the current owner of the ball
    /// </summary>
    public BallOwnerType GetCurrentOwner()
    {
        return _currentOwner;
    }
    
    /// <summary>
    /// Resets the player interaction timer
    /// </summary>
    public void ResetTimer()
    {
        _lastPlayerInteractionTime = Time.time;
    }
}
