using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages game state and modes
/// </summary>
public class GameManager : MonoBehaviour
{
    // Events for music system
    public event Action OnGameStarted;
    public event Action OnReturnToMenu;

    // Singleton instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            // Don't create a new instance during cleanup/quit
            if (_instance == null && !ApplicationQuit)
            {
                _instance = FindObjectOfType<GameManager>();
                
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private static bool ApplicationQuit = false;

    // Game state
    public bool isGameActive { get; private set; }
    public bool isTutorialMode { get; private set; } = false;
    
    // Tutorial settings
    [Header("Tutorial Settings")]
    [SerializeField] private float tutorialBallSpeedMultiplier = 0.3f;
    
    [Header("Game Elements")]
    [SerializeField] private GameObject playerContainer;
    [SerializeField] private GameObject bossContainer;
    [SerializeField] private GameObject ballContainer;
    [SerializeField] private List<GameObject> additionalGameElements = new List<GameObject>();
    
    [Header("UI Elements")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject tutorialUI;
    private UIFadeManager _fadeManager;
    
    [Header("Game Start Animation")]
    [SerializeField] private Animator gameStartAnimator;
    [SerializeField] private string startAnimationTrigger = "StartGame";
    [SerializeField] private string tutorialAnimationTrigger = "StartTutorial";
    
    // Properties
    public float TutorialBallSpeedMultiplier => tutorialBallSpeedMultiplier;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Find or create UIFadeManager
            _fadeManager = FindObjectOfType<UIFadeManager>();
            if (_fadeManager == null)
            {
                GameObject fadeManagerObj = new GameObject("UIFadeManager");
                _fadeManager = fadeManagerObj.AddComponent<UIFadeManager>();
                DontDestroyOnLoad(fadeManagerObj);
            }
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        ApplicationQuit = true;
    }

    private void Start()
    {
        // Game should be inactive at start, with only the main menu shown
        DisableGameElements();
        ShowMainMenu(true); // true for instant show on game start
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset game state when a new scene is loaded
        isGameActive = false;
        
        // Find game elements if they're not assigned
        FindGameElements();
        
        // Disable game elements and show main menu
        DisableGameElements();
        ShowMainMenu(true); // true for instant show on scene load
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
            
        // Find UI elements if they're not already assigned
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
        
        // Set up UI with fade transitions
        if (mainMenuUI != null) _fadeManager.HideUI(mainMenuUI);
        if (gameplayUI != null) _fadeManager.ShowUI(gameplayUI);
        if (tutorialUI != null) _fadeManager.HideUI(tutorialUI);
        
        // Hide cursor during gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Enable game elements
        EnableGameElements();
        
        // Now that everything is set up, mark the game as active
        isGameActive = true;
        
        // Notify listeners that game has started
        OnGameStarted?.Invoke();
        
        // Start the animation if available
        if (gameStartAnimator != null)
        {
            gameStartAnimator.gameObject.SetActive(true);
            gameStartAnimator.SetTrigger(startAnimationTrigger);
        }
    }

    public void QuitGame(){
        Application.Quit();
    }

    /// <summary>
    /// Start the
    /// game in tutorial mode
    /// </summary>
    public void StartTutorial()
    {
        isTutorialMode = true;
        
        // Make sure all game elements are found
        FindGameElements();
        
        // Set up UI with fade transitions
        if (mainMenuUI != null) _fadeManager.HideUI(mainMenuUI);
        if (gameplayUI != null) _fadeManager.ShowUI(gameplayUI);
        if (tutorialUI != null) _fadeManager.ShowUI(tutorialUI);
        
        // Keep cursor visible for tutorial UI
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Enable game elements
        EnableGameElements();
        
        // Now that everything is set up, mark the game as active
        isGameActive = true;
        
        // Notify listeners that game has started
        OnGameStarted?.Invoke();
        
        // Start the animation if available
        if (gameStartAnimator != null)
        {
            gameStartAnimator.gameObject.SetActive(true);
            gameStartAnimator.SetTrigger(tutorialAnimationTrigger);
        }
    }
    
    /// <summary>
    /// Called by animation event when the start animation is complete
    /// </summary>
    public void AnimationCompleted()
    {
        // Enable gameplay elements
        EnableGameElements();
        
        // Hide the animation GameObject
        if (gameStartAnimator != null)
        {
            gameStartAnimator.gameObject.SetActive(false);
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
        
        // Show main menu UI with fade transitions
        ShowMainMenu();
        
        // Notify listeners we're returning to menu
        OnReturnToMenu?.Invoke();
    }

    /// <summary>
    /// Show the main menu UI
    /// </summary>
    private void ShowMainMenu(bool instant = false)
    {
        // Show main menu UI and hide other UIs with fade transitions
        if (mainMenuUI != null) _fadeManager.ShowUI(mainMenuUI, instant);
        if (gameplayUI != null) _fadeManager.HideUI(gameplayUI, instant);
        if (tutorialUI != null) _fadeManager.HideUI(tutorialUI, instant);
    }
    
    /// <summary>
    /// Enable all game elements
    /// </summary>
    private void EnableGameElements()
    {
        if (playerContainer != null) playerContainer.SetActive(true);
        if (bossContainer != null) bossContainer.SetActive(true);
        if (ballContainer != null) ballContainer.SetActive(true);
        
        foreach (var element in additionalGameElements)
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
        
        foreach (var element in additionalGameElements)
        {
            if (element != null)
                element.SetActive(false);
        }
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