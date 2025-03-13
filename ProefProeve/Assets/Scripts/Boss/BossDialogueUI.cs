using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Helper class to configure the Boss Dialogue UI
/// </summary>
public class BossDialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform backgroundPanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Appearance")]
    [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Vector2 padding = new Vector2(40f, 20f);
    [SerializeField] private float maxWidth = 400f;
    
    private RectTransform textRectTransform;
    private ContentSizeFitter contentSizeFitter;
    private string currentText = "";

    // Event to notify when text is hidden
    public event System.Action OnBackspaceComplete;

    private void Awake()
    {
        // Make sure our components are set up
        ConfigureUI();
    }
    
    public void ConfigureUI()
    {
        // Set up the text component if needed
        if (dialogueText == null)
        {
            dialogueText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (dialogueText != null)
        {
            // Configure text settings
            dialogueText.color = textColor;
            dialogueText.enableAutoSizing = false;
            dialogueText.enableWordWrapping = true;
            
            // Initially clear any text
            dialogueText.text = "";
            currentText = "";
            
            // Get the rect transform for layout adjustments
            textRectTransform = dialogueText.GetComponent<RectTransform>();
            if (textRectTransform != null)
            {
                // Set max width for wrapping
                textRectTransform.sizeDelta = new Vector2(maxWidth, textRectTransform.sizeDelta.y);
            }
            
            // Add content size fitter if needed
            contentSizeFitter = dialogueText.GetComponent<ContentSizeFitter>();
            if (contentSizeFitter == null)
            {
                contentSizeFitter = dialogueText.gameObject.AddComponent<ContentSizeFitter>();
            }
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // Set up the background if assigned
        if (backgroundPanel != null)
        {
            Image panelImage = backgroundPanel.GetComponent<Image>();
            if (panelImage == null)
            {
                panelImage = backgroundPanel.gameObject.AddComponent<Image>();
            }
            panelImage.color = backgroundColor;
        }
    }
    
    public void UpdateDialogueBoxSize()
    {
        if (backgroundPanel == null || textRectTransform == null)
            return;
        
        // Force layout update 
        LayoutRebuilder.ForceRebuildLayoutImmediate(textRectTransform);
        
        // Adjust background to fit text with padding
        Vector2 textSize = textRectTransform.sizeDelta;
        
        // Handle empty text case
        if (string.IsNullOrEmpty(dialogueText.text))
        {
            backgroundPanel.sizeDelta = new Vector2(maxWidth, 0);
        }
        else
        {
            backgroundPanel.sizeDelta = new Vector2(
                Mathf.Min(textSize.x + padding.x, maxWidth),
                textSize.y + padding.y
            );
        }
    }
    
    /// <summary>
    /// Set text and store it for animations
    /// </summary>
    public void SetText(string text)
    {
        if (dialogueText == null) return;
        
        currentText = text;
        dialogueText.text = text;
        UpdateDialogueBoxSize();
    }
    
    /// <summary>
    /// Hide the text immediately
    /// </summary>
    public void StartBackspaceEffect()
    {
        if (dialogueText != null)
        {
            currentText = "";
            dialogueText.text = "";
            UpdateDialogueBoxSize();
            OnBackspaceComplete?.Invoke();
        }
    }
}