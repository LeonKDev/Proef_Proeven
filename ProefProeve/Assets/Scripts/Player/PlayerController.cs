using System;
using System.Collections.Generic;
using UnityEngine;
using BallGame.Shared;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    [Header("Ball Interaction")]
    [SerializeField] private float maxHitDistance = 4f;
    [SerializeField] private float perfectHitThreshold = 0.3f;
    [SerializeField] private float normalBounceMultiplier = 2f;
    [SerializeField] private float closeRangeBounceMultiplier = 3.5f;
    
    [Header("References")]
    [SerializeField] private GameObject bossObject;
    [SerializeField] private PlayerBatHandler batHandler;
    
    [Header("Debug")]
    [SerializeField] private bool allowSwingWithoutBall = false;
    
    // Store all active balls in the scene
    private List<BallController> _activeBalls = new List<BallController>();
    
    private void Start()
    {
        // Register this player with the ball registration manager
        if (BallRegistrationManager.Instance != null)
        {
            BallRegistrationManager.Instance.RegisterPlayer(this);
        }
        
        // Find boss object if not assigned
        if (bossObject == null)
        {
            bossObject = GameObject.FindGameObjectWithTag("Boss");
            if (bossObject == null)
            {
                Debug.LogWarning("Boss reference not set and could not be found with tag");
            }
        }
    }

    private void OnDestroy()
    {
        // Unregister from the ball registration manager when destroyed
        if (BallRegistrationManager.Instance != null)
        {
            BallRegistrationManager.Instance.UnregisterPlayer(this);
        }
    }
    
    private void Update()
    {
        // Check for bat hit input
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleBallHit();
        }
    }
    
    /// <summary>
    /// Refreshes the list of active balls in the scene
    /// </summary>
    public void RefreshBallsList()
    {
        _activeBalls.Clear();
        BallController[] balls = FindObjectsOfType<BallController>();
        _activeBalls.AddRange(balls);
    }
    
    /// <summary>
    /// Adds a ball to the active balls list
    /// </summary>
    public void AddBall(BallController ball)
    {
        if (ball != null && !_activeBalls.Contains(ball))
        {
            _activeBalls.Add(ball);
            Debug.Log($"Added ball to player's active balls list. Total balls: {_activeBalls.Count}");
        }
    }
    
    /// <summary>
    /// Removes a ball from the active balls list
    /// </summary>
    public void RemoveBall(BallController ball)
    {
        if (ball != null)
        {
            _activeBalls.Remove(ball);
            Debug.Log($"Removed ball from player's active balls list. Remaining balls: {_activeBalls.Count}");
        }
    }
    
    private void HandleBallHit()
    {
        // Find the closest ball that's in range
        BallController closestBall = FindClosestBallInRange();
        
        if (closestBall != null)
        {
            // Get the distance to the closest ball
            float distanceToPlayer = Vector3.Distance(closestBall.transform.position, transform.position);
            
            // Swing the bat
            if (batHandler != null)
            {
                batHandler.SwingBat();
            }
            
            Vector3 directionToUse;
            float bounceMultiplier;
            
            // Check if this is a perfect hit (very close to max distance)
            if (distanceToPlayer >= maxHitDistance - perfectHitThreshold && bossObject != null)
            {
                // Perfect hit! Direct towards the boss
                directionToUse = (bossObject.transform.position - closestBall.transform.position).normalized;
                bounceMultiplier = normalBounceMultiplier * 1.5f; // Extra power for perfect hit
                closestBall.SetPerfectHit(true);
                
                Debug.Log("Perfect hit! Ball directed toward boss.");
            }
            else
            {
                // Regular hit - use player's forward direction
                directionToUse = transform.forward;
                bounceMultiplier = distanceToPlayer <= 2f ? closeRangeBounceMultiplier : normalBounceMultiplier;
                closestBall.SetPerfectHit(false);
            }
            
            // Tell the ball to apply the boost
            closestBall.GetBoostHandler().ApplyBatBoost(directionToUse, bounceMultiplier);
            
            // Update owner to Player
            BallOwnerHandler ownerHandler = closestBall.GetComponent<BallOwnerHandler>();
            if (ownerHandler != null)
            {
                ownerHandler.SetOwner(BallOwnerType.Player);
            }
        }
        else if (allowSwingWithoutBall)
        {
            // Only swing the bat when no ball is in range if explicitly enabled
            if (batHandler != null)
            {
                batHandler.SwingBat();
                Debug.Log("Swinging bat with no ball in range (debug mode)");
            }
        }
        else
        {
            // No ball in range - don't allow swing
            Debug.Log("Can't swing - no ball in range");
        }
    }
    
    /// <summary>
    /// Finds the closest ball that's within the maximum hit distance
    /// </summary>
    /// <returns>The closest ball controller or null if none in range</returns>
    private BallController FindClosestBallInRange()
    {
        BallController closestBall = null;
        float closestDistance = maxHitDistance;
        
        foreach (BallController ball in _activeBalls)
        {
            if (ball != null)
            {
                float distance = Vector3.Distance(ball.transform.position, transform.position);
                
                if (distance <= maxHitDistance && distance < closestDistance)
                {
                    closestBall = ball;
                    closestDistance = distance;
                }
            }
        }
        
        return closestBall;
    }

    
}