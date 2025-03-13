using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a visual indicator when in tutorial mode
/// </summary>
public class TutorialIndicator : MonoBehaviour
{
    [SerializeField] private GameObject tutorialIndicatorObject;
    [SerializeField] private Text tutorialText;
    [SerializeField] private string tutorialMessage = "TUTORIAL MODE";
    
    private void Start()
    {
        // Check if we're in tutorial mode
        bool isTutorial = GameManager.Instance != null && GameManager.Instance.isTutorialMode;
        
        // Show or hide the indicator based on mode
        if (tutorialIndicatorObject != null)
            tutorialIndicatorObject.SetActive(isTutorial);
            
        // Set tutorial text if available
        if (tutorialText != null && isTutorial)
            tutorialText.text = tutorialMessage;
    }
}