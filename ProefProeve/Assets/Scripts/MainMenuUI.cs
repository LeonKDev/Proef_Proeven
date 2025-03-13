using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the main menu UI displayed within the game scene
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button exitButton;
    
    [Header("UI Settings")]
    [SerializeField] private string playButtonText = "START GAME";
    [SerializeField] private string tutorialButtonText = "TUTORIAL";
    [SerializeField] private string exitButtonText = "EXIT";
    
    private void Awake()
    {
        // Make sure this GameObject is tagged properly for the GameManager to find
        if (gameObject.tag != "MainMenuUI")
        {
            gameObject.tag = "MainMenuUI";
            Debug.Log("MainMenuUI: Tagged as 'MainMenuUI' for GameManager to find");
        }
    }
    
    private void Start()
    {
        // Set up button texts if there are Text components attached
        SetupButtonText(playButton, playButtonText);
        SetupButtonText(tutorialButton, tutorialButtonText);
        SetupButtonText(exitButton, exitButtonText);
        
        // Add button listeners
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
            
        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(OnTutorialClicked);
            
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
            
        // Make sure cursor is visible for menu interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    
    private void SetupButtonText(Button button, string text)
    {
        if (button != null)
        {
            Text buttonText = button.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }
    }
    
    private void OnPlayClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogError("MainMenuUI: GameManager instance not found!");
        }
    }
    
    private void OnTutorialClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartTutorial();
        }
        else
        {
            Debug.LogError("MainMenuUI: GameManager instance not found!");
        }
    }
    
    private void OnExitClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void OnDestroy()
    {
        // Clean up button listeners
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayClicked);
            
        if (tutorialButton != null)
            tutorialButton.onClick.RemoveListener(OnTutorialClicked);
            
        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);
    }
}