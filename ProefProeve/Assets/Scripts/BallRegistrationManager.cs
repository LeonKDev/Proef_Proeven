using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manager that handles ball registration with players
/// This helps with dynamically adding and removing balls from the scene
/// </summary>
public class BallRegistrationManager : MonoBehaviour
{
    private static BallRegistrationManager _instance;
    public static BallRegistrationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("BallRegistrationManager");
                _instance = go.AddComponent<BallRegistrationManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }
    
    private List<PlayerController> _players = new List<PlayerController>();
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
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
}