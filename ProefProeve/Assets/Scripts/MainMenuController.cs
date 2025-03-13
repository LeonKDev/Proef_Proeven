using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu UI and navigation
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button exitButton;
    
    private void Start()
    {
        // Add listeners to buttons
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        
        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(OnTutorialButtonClicked);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitButtonClicked);
    }
    
    private void OnPlayButtonClicked()
    {
        // Start regular game
        GameManager.Instance.StartGame();
    }
    
    private void OnTutorialButtonClicked()
    {
        // Start tutorial mode
        GameManager.Instance.StartTutorial();
    }
    
    private void OnExitButtonClicked()
    {
        // Quit the application
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
        
        if (tutorialButton != null)
            tutorialButton.onClick.RemoveListener(OnTutorialButtonClicked);
        
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }
}