using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

/// <summary>
/// Manages game state and modes
/// </summary>
public class GameManager : MonoBehaviour
{
    // Events for music system
    public event Action OnGameStarted;
    public event Action OnReturnToMenu;

    // Singleton instance that refers to the scene instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null && !ApplicationQuit)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    private static bool ApplicationQuit = false;

    // Game state
    public bool isGameActive { get; private set; }
    public bool isTutorialMode { get; private set; } = false;
    private bool isVictoryState = false;
    
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
    public GameObject[] victoryObjects; // Made public to allow PlayerHealth to access it
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
            
            // Find UIFadeManager
            _fadeManager = FindObjectOfType<UIFadeManager>();
            if (_fadeManager == null)
            {
                GameObject fadeManagerObj = new GameObject("UIFadeManager");
                _fadeManager = fadeManagerObj.AddComponent<UIFadeManager>();
            }
        }
        else if (_instance != this)
        {
            // If there's already an instance, destroy this one
            Destroy(gameObject);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        ApplicationQuit = true;
    }

    private void Start()
    {
        // Game should be inactive at start, with only the main menu shown
        DisableGameElements();
        HideVictoryObjects();
        ShowMainMenu(true); // true for instant show on game start
    }

    private void Update()
    {
        // Check for restart input in victory state
        if (isVictoryState)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ReturnToMainMenu();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset game state when a new scene is loaded
        isGameActive = false;
        
        // Find game elements if they're not assigned
        FindGameElements();
        
        // Disable game elements and show main menu
        DisableGameElements();
        HideVictoryObjects();
        ShowMainMenu(true); // true for instant show on scene load

        // Reset cursor state
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
    /// Start the game in tutorial mode
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
        isVictoryState = false;

        // Notify listeners we're returning to menu before reloading
        OnReturnToMenu?.Invoke();
        
        // Get current scene name and reload it
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    private void ClearPersistentObjects()
    {
        // Find and destroy any persistent game objects that shouldn't survive reload
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // Don't destroy the GameManager itself or the MusicManager
            if (obj != gameObject && obj.GetComponent<MusicManager>() == null)
            {
                // Check if the object is marked DontDestroyOnLoad
                Scene objScene = obj.scene;
                if (objScene.buildIndex == -1) // DontDestroyOnLoad objects are in a special scene
                {
                    Destroy(obj);
                }
            }
        }
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

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus && isGameActive && !isTutorialMode)
        {
            // Hide cursor when game window gains focus during gameplay
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // Show cursor when window loses focus or in menu/tutorial
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    /// <summary>
    /// Called when the boss is defeated to hide game elements
    /// </summary>
    public void HandleBossDefeated()
    {
        // Hide UI
        if (gameplayUI != null) _fadeManager.HideUI(gameplayUI);
        if (tutorialUI != null) _fadeManager.HideUI(tutorialUI);
        
        // Hide game elements
        DisableGameElements();
        
        // Show victory objects
        ShowVictoryObjects();
        
        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Set victory state
        isVictoryState = true;
    }

    /// <summary>
    /// Called when the player dies to show game over screen
    /// </summary>
    public void HandlePlayerDeath()
    {
        // Hide UI
        if (gameplayUI != null) _fadeManager.HideUI(gameplayUI);
        if (tutorialUI != null) _fadeManager.HideUI(tutorialUI);
        
        // Hide game elements
        DisableGameElements();
        
        // Show victory/game over objects
        ShowVictoryObjects();
        
        // Update the victory text to say "Game Over"
        if (victoryObjects != null)
        {
            foreach (var obj in victoryObjects)
            {
                if (obj != null)
                {
                    TextMeshProUGUI[] texts = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI text in texts)
                    {
                        if (text.text.Contains("Victory") || text.text.Contains("Game End"))
                        {
                            text.text = "Game Over";
                        }
                    }
                }
            }
        }
        
        // Show cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Set victory state (this enables the return to menu input)
        isVictoryState = true;
    }

    private void ShowVictoryObjects()
    {
        if (victoryObjects != null)
        {
            foreach (var obj in victoryObjects)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }

    private void HideVictoryObjects()
    {
        if (victoryObjects != null)
        {
            foreach (var obj in victoryObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    public void PlayHitFlash(Renderer renderer, Color flashColor)
    {
        if (renderer != null)
        {
            StartCoroutine(HitFlashCoroutine(renderer, flashColor));
        }
    }

    private IEnumerator HitFlashCoroutine(Renderer renderer, Color flashColor)
    {
        if (renderer == null) yield break;
        
        var material = renderer.material;
        Color originalColor = material.color;
        
        material.color = flashColor;     
        yield return new WaitForSeconds(0.5f); 
        material.color = originalColor;     
        yield return new WaitForSeconds(0.5f);
        material.color = flashColor;     
        yield return new WaitForSeconds(0.5f); 
        material.color = originalColor;     
        yield return new WaitForSeconds(0.5f);
    }
}