using UnityEngine;

/// <summary>
/// Main player class that acts as a hub for all player-related components
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Component References")]
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerBatHandler batHandler;
    
    [Header("External References")]
    [SerializeField] private GameObject bossObject;
    
    private void Awake()
    {
        // Only set the tag for targeting purposes
        if (gameObject.tag != "Player")
        {
            gameObject.tag = "Player";
        }
    }
    
    // Accessor methods for components
    public PlayerController GetController() => controller;
    public PlayerBatHandler GetBatHandler() => batHandler;
}