using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Manages game state and modes
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                _instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Game state
    public bool isGameActive { get; private set; }
    public bool isTutorialMode { get; private set; } = false;
    
    // Tutorial settings
    [Header("Tutorial Settings")]
    [SerializeField] private float tutorialBallSpeedMultiplier = 0.3f; // Ball moves at 30% speed in tutorial
    
    [Header("Game Elements")]
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private GameObject bossContainer;
    [SerializeField] private GameObject ballContainer;
    [SerializeField] private List<GameObject> additionalGameElements = new List<GameObject>();
    
    [Header("UI Elements")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject tutorialUI;
    
    [Header("Game Start Animation")]
    [SerializeField] private Animator gameStartAnimator;
    [SerializeField] private string startAnimationTrigger = "StartGame";
    [SerializeField] private string tutorialAnimationTrigger = "StartTutorial";
    
    /// <summary>
    /// There is an animation in progress that we are currently blocking - it needs an instance variable
    /// to keep track of it. We are not making monobehavior classes static! </summary>
    private bool animationInProgress = false;
    
    // Properties
    public float TutorialBallSpeedMultiplier => tutorialBallSpeedMultiplier;

    private void Awake()
    {
        // Singleton pattern setup
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Game should be inactive at start, with only the main menu shown
        DisableGameElements();
        ShowMainMenu();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset game state when a new scene is loaded
        isGameActive = false;
        
        // Find game elements if they're not assigned
        FindGameElements();
        
        // Disable game elements and show main menu
        DisableGameElements();
        ShowMainMenu();
    }
    
    /// <summary>
    /// Find all necessary game elements in the scene
    /// </summary>
    private void FindGameElements()
    {
        Debug.Log("Finding game elements...");

        // Try to find elements by tag first, then by name if tag search fails
        if (playerContainer == null) {
            playerContainer = GameObject.FindGameObjectWithTag("PlayerContainer");
            // If tag search fails, try by name
            if (playerContainer == null) {
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects) {
                    if (obj.name.Contains("PlayerContainer")) {
                        playerContainer = obj;
                        Debug.Log("Found player container by name: " + obj.name);
                        break;
                    }
                }
            }
        }
            
        if (bossContainer == null) {
            bossContainer = GameObject.FindGameObjectWithTag("BossContainer");
            // If tag search fails, try by name
            if (bossContainer == null) {
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects) {
                    if (obj.name.Contains("BossContainer")) {
                        bossContainer = obj;
                        Debug.Log("Found boss container by name: " + obj.name);
                        break;
                    }
                }
            }
        }
            
        if (ballContainer == null) {
            ballContainer = GameObject.FindGameObjectWithTag("BallContainer");
            // If tag search fails, try by name
            if (ballContainer == null) {
                GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects) {
                    if (obj.name.Contains("BallContainer")) {
                        ballContainer = obj;
                        Debug.Log("Found ball container by name: " + obj.name);
                        break;
                    }
                }
            }
        }
            
        // Find UI elements if they're not already assigned (keep existing code)
        if (mainMenuUI == null)
            mainMenuUI = GameObject.FindGameObjectWithTag("MainMenuUI");
            
        if (gameplayUI == null)
            gameplayUI = GameObject.FindGameObjectWithTag("GameplayUI");
            
        if (tutorialUI == null)
            tutorialUI = GameObject.FindGameObjectWithTag("TutorialUI");
            
        // Log the state of found elements
        Debug.Log("After search - PlayerContainer: " + (playerContainer != null) + 
                 ", BossContainer: " + (bossContainer != null) + 
                 ", BallContainer: " + (ballContainer != null));
    }
    
    /// <summary>
    /// Start the game in normal mode
    /// </summary>
    public void StartGame()
    {
        isTutorialMode = false;
        
        // Make sure all game elements are found
        FindGameElements();
        
        // Hide the main menu UI immediately
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        
        // Enable game elements immediately
        EnableGameElements();
        isGameActive = true;
        
        // Start the animation if available
        if (gameStartAnimator != null)
        {
            animationInProgress = true;
            gameStartAnimator.gameObject.SetActive(true);
            gameStartAnimator.SetTrigger(startAnimationTrigger);
            
            // The animation will call AnimationCompleted() via an Animation Event when done
        }
    }
    
    /// <summary>
    /// Start the game in tutorial mode
    /// </summary>
    public void StartTutorial()
    {
        isTutorialMode = true;
        
        // Make sure all game elements are found
        FindGameElements();
        
        // Hide the main menu UI immediately
        if (mainMenuUI != null) mainMenuUI.SetActive(false);
        
        // Enable game elements immediately
        EnableGameElements();
        isGameActive = true;
        
        // Start the animation if available
        if (gameStartAnimator != null)
        {
            animationInProgress = true;
            gameStartAnimator.gameObject.SetActive(true);
            gameStartAnimator.SetTrigger(tutorialAnimationTrigger);
            
            // The animation will call AnimationCompleted() via an Animation Event when done
        }
    }
    
    /// <summary>
    /// Called by animation event when the start animation is complete
    /// </summary>
    public void AnimationCompleted()
    {
        animationInProgress = false;
        
        // Make sure all game elements are found
        FindGameElements();
        
        // Now activate the game
        ActivateGameAfterAnimation();
        
        // Hide the animation GameObject if it's still showing
        if (gameStartAnimator != null)
        {
            gameStartAnimator.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Activates the game after the animation is complete
    /// </summary>
    private void ActivateGameAfterAnimation()
    {
        // Debug log to check activation sequence
        Debug.Log("ActivateGameAfterAnimation called, about to enable game elements");
        
        // Only proceed if there's no animation in progress
        if (!animationInProgress)
        {
            // Enable game elements first
            EnableGameElements();
            
            // Then set game as active
            isGameActive = true;
            
            // Set cursor state based on game mode
            if (isTutorialMode)
            {
                // Keep cursor visible in tutorial for UI interaction
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                // Show tutorial UI
                if (gameplayUI != null) gameplayUI.SetActive(true);
                if (tutorialUI != null) tutorialUI.SetActive(true);
            }
            else
            {
                // Hide cursor during normal gameplay
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                
                // Show gameplay UI
                if (gameplayUI != null) gameplayUI.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("Attempted to activate game while animation is in progress. Activation postponed until animation completes.");
        }
    }
    
    /// <summary>
    /// Return to the main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        isTutorialMode = false;
        isGameActive = false;
        
        // Disable game elements
        DisableGameElements();
        
        // Show cursor for menu interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Show main menu UI
        ShowMainMenu();
    }
    
    /// <summary>
    /// Enable all game elements
    /// </summary>
    private void EnableGameElements()
    {
        // Debug log to check if containers are being found
        Debug.Log("Enabling game elements - Player: " + (playerContainer != null) + 
                  ", Boss: " + (bossContainer != null) + 
                  ", Ball: " + (ballContainer != null));

        if (playerContainer != null) playerContainer.SetActive(true);
        if (bossContainer != null) bossContainer.SetActive(true);
        if (ballContainer != null) ballContainer.SetActive(true);
        
        // Enable additional game elements
        foreach (GameObject element in additionalGameElements)
        {
            if (element != null)
                element.SetActive(true);
        }
    }
    
    /// <summary>
    /// Disable all game elements
    /// </summary>
    private void DisableGameElements()
    {
        if (playerContainer != null) playerContainer.SetActive(false);
        if (bossContainer != null) bossContainer.SetActive(false);
        if (ballContainer != null) ballContainer.SetActive(false);
        
        // Disable additional game elements
        foreach (GameObject element in additionalGameElements)
        {
            if (element != null)
                element.SetActive(false);
        }
    }
    
    /// <summary>
    /// Show the main menu UI
    /// </summary>
    private void ShowMainMenu()
    {
        // Show cursor for menu interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Show main menu UI and hide other UIs
        if (mainMenuUI != null) mainMenuUI.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (tutorialUI != null) tutorialUI.SetActive(false);
    }
    
    // Add any game element that should be controlled by the game manager
    public void AddGameElement(GameObject element)
    {
        if (element != null && !additionalGameElements.Contains(element))
            additionalGameElements.Add(element);
    }
    
    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}