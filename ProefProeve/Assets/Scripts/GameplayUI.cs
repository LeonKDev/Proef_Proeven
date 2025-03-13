using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the gameplay UI elements, including pause and menu buttons
/// </summary>
public class GameplayUI : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button menuButton;
    
    private void Awake()
    {
        // Tag this object for the GameManager to find
        if (gameObject.tag != "GameplayUI")
        {
            gameObject.tag = "GameplayUI";
        }
    }
    
    private void Start()
    {
        // Set up button listeners
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuButtonClicked);
        }
    }
    
    private void OnMenuButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            // Return to main menu
            GameManager.Instance.ReturnToMainMenu();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up listeners
        if (menuButton != null)
        {
            menuButton.onClick.RemoveListener(OnMenuButtonClicked);
        }
    }
}