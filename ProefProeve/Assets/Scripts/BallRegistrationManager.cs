using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Manager that handles ball registration with players
/// This helps with dynamically adding and removing balls from the scene
/// </summary>
public class BallRegistrationManager : MonoBehaviour
{
    // Static instance - properly initialized in Awake
    private static BallRegistrationManager _instance;
    
    // Property with improved instance access
    public static BallRegistrationManager Instance
    {
        get
        {
            // Only block access after application has explicitly quit
            if (_applicationIsQuitting)
            {
                Debug.LogWarning("[BallRegistrationManager] Instance requested after application quit. Returning null.");
                return null;
            }
            
            return _instance;
        }
    }
    
    // Flag to prevent accessing Instance during application quit
    private static bool _applicationIsQuitting = false;
    
    private List<PlayerController> _players = new List<PlayerController>();
    
    private void Awake()
    {
        // Standard singleton initialization
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // Subscribe to scene events
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            Debug.Log("[BallRegistrationManager] Instance initialized.");
        }
        else if (_instance != this)
        {
            // If an instance already exists, destroy this one
            Debug.Log("[BallRegistrationManager] Duplicate instance found and destroyed.");
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Find all existing players on start
        RefreshPlayerList();
    }
    
    /// <summary>
    /// Refreshes the list of active players
    /// </summary>
    public void RefreshPlayerList()
    {
        _players.Clear();
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        _players.AddRange(players);
    }
    
    /// <summary>
    /// Registers a player with the manager
    /// </summary>
    public void RegisterPlayer(PlayerController player)
    {
        if (player != null && !_players.Contains(player))
        {
            _players.Add(player);
        }
    }
    
    /// <summary>
    /// Unregisters a player from the manager
    /// </summary>
    public void UnregisterPlayer(PlayerController player)
    {
        if (player != null)
        {
            _players.Remove(player);
        }
    }
    
    /// <summary>
    /// Registers a ball with all active players
    /// </summary>
    public void RegisterBall(BallController ball)
    {
        if (ball == null) return;
        
        foreach (var player in _players)
        {
            player.AddBall(ball);
        }
    }
    
    /// <summary>
    /// Unregisters a ball from all active players
    /// </summary>
    public void UnregisterBall(BallController ball)
    {
        if (ball == null) return;
        
        foreach (var player in _players)
        {
            player.RemoveBall(ball);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Refresh player list when new scene loads
        RefreshPlayerList();
    }
    
    private void OnSceneUnloaded(Scene scene)
    {
        // Clear player list when scene unloads
        _players.Clear();
    }
    
    private void OnDestroy()
    {
        // Clean up listeners and reset instance if this is the current instance
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            _instance = null;
            Debug.Log("[BallRegistrationManager] Instance destroyed.");
        }
    }
    
    private void OnApplicationQuit()
    {
        // Set flag to prevent access during application quit
        _applicationIsQuitting = true;
        _instance = null;
        Debug.Log("[BallRegistrationManager] Application quitting, instance nullified.");
    }
}