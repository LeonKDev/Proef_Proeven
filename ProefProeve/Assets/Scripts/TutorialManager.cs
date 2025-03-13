using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the tutorial mode functionality and UI
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button exitTutorialButton;
    
    [Header("Tutorial Settings")]
    [SerializeField] private string[] tutorialInstructions;
    [SerializeField] private float instructionDisplayTime = 5f;
    
    private int currentInstructionIndex = 0;
    private float timeUntilNextInstruction;
    
    private void Start()
    {
        // Only activate if we're in tutorial mode
        if (GameManager.Instance == null || !GameManager.Instance.isTutorialMode)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Set up the UI
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);
            
        // Configure exit button
        if (exitTutorialButton != null)
            exitTutorialButton.onClick.AddListener(OnExitTutorialClicked);
        
        // Start the tutorial sequence
        ShowCurrentInstruction();
        timeUntilNextInstruction = instructionDisplayTime;
    }
    
    private void Update()
    {
        // Only proceed if we're in tutorial mode
        if (GameManager.Instance == null || !GameManager.Instance.isTutorialMode)
            return;
            
        // Update instruction timing
        if (currentInstructionIndex < tutorialInstructions.Length - 1)
        {
            timeUntilNextInstruction -= Time.deltaTime;
            
            if (timeUntilNextInstruction <= 0)
            {
                currentInstructionIndex++;
                ShowCurrentInstruction();
                timeUntilNextInstruction = instructionDisplayTime;
            }
        }
    }
    
    private void ShowCurrentInstruction()
    {
        if (instructionText != null && tutorialInstructions.Length > 0 && 
            currentInstructionIndex < tutorialInstructions.Length)
        {
            instructionText.text = tutorialInstructions[currentInstructionIndex];
        }
    }
    
    private void OnExitTutorialClicked()
    {
        // Return to the main menu
        GameManager.Instance.ReturnToMainMenu();
    }
    
    private void OnDestroy()
    {
        // Clean up listeners
        if (exitTutorialButton != null)
            exitTutorialButton.onClick.RemoveListener(OnExitTutorialClicked);
    }
}