using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Reflection;

public class BossDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private RectTransform backgroundRect;
    [SerializeField] private Vector2 padding = new Vector2(40f, 20f); // Padding for text box
    
    [Header("Timing Settings")]
    [SerializeField] private float messageDisplayTime = 10f; // How long each message displays
    [SerializeField] private float messageCooldownTime = 10f; // Time between messages disappearing and new ones appearing
    [SerializeField] private bool showMessageOnStart = true; // Show a message immediately when activated
    
    [Header("Sound Settings")]
    [SerializeField] private AudioClip dialogueStartSound;
    [SerializeField] private AudioClip typingSound;
    [SerializeField] private AudioSource typingSoundSource;
    [Range(0f, 1f)]
    [SerializeField] private float typingSoundVolume = 0.5f;
    [Range(0.8f, 1.2f)]
    [SerializeField] private float typingSoundPitchVariation = 1.1f;

    [Header("Text Animation")]
    [SerializeField] private float typingSpeed = 0.05f; // Seconds per character
    [SerializeField] private float delayAfterPunctuation = 0.2f; // Additional delay after punctuation
    [SerializeField] private bool useTypewriterEffect = true; // Toggle for the typewriter effect
    
    [Header("ChatGPT Integration")]
    [SerializeField] private string[] fallbackMessages = new string[] 
    {
        "I will crush you!",
        "You cannot defeat me!",
        "Your skills are no match for mine!",
        "Prepare to be destroyed!",
        "Is that all you've got?",
        "This battle is pointless. Give up now!",
        "I've faced tougher opponents than you!",
        "You're just delaying the inevitable...",
        "Your determination is amusing but futile.",
        "Soon this will all be over!"
    };

    // For API integration - use the existing ChatGPTIntegration
    private ChatGPTIntegration chatGPTIntegration;
    
    private float timer = 0f;
    private bool isMessageDisplaying = false;
    private enum DialogueState { Waiting, Displaying, Typing }
    private DialogueState currentState = DialogueState.Waiting;
    private Queue<string> messageQueue = new Queue<string>();
    
    // Layout components
    private ContentSizeFitter textSizeFitter;
    private LayoutElement textLayoutElement;
    
    // Typewriter effect
    private Coroutine typewriterCoroutine;
    private string currentFullMessage = "";

    private void OnEnable()
    {
        Debug.Log("BossDialogueManager enabled");
        
        // Check for required references
        if (dialogueBox == null || dialogueText == null)
        {
            Debug.LogError("Required UI references are missing in BossDialogueManager. Please assign them in the Inspector.");
            return;
        }
        
        // Set up text for auto-sizing if not already done
        SetupTextAutoSizing();
        
        // Hide dialogue box initially
        dialogueBox.SetActive(false);
                
        // Show an initial message when the boss dialogue system is enabled
        if (showMessageOnStart)
        {
            Debug.Log("Queuing initial message");
            QueueRandomMessage();
            // Start in waiting state with 0 timer to show message quickly
            currentState = DialogueState.Waiting;
            timer = messageCooldownTime - 1f; // Show message after 1 second
        }
    }

    private void SetupTextAutoSizing()
    {
        // Add content size fitter if needed
        if (textSizeFitter == null)
        {
            textSizeFitter = dialogueText.gameObject.GetComponent<ContentSizeFitter>();
            if (textSizeFitter == null)
            {
                textSizeFitter = dialogueText.gameObject.AddComponent<ContentSizeFitter>();
            }
            textSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        
        // Add layout element if needed
        if (textLayoutElement == null)
        {
            textLayoutElement = dialogueText.gameObject.GetComponent<LayoutElement>();
            if (textLayoutElement == null)
            {
                textLayoutElement = dialogueText.gameObject.AddComponent<LayoutElement>();
            }
        }
        
        // Configure text
        dialogueText.enableAutoSizing = true;
        dialogueText.enableWordWrapping = true;
    }

    private void Start()
    {
        Debug.Log("BossDialogueManager started");
            
        // Initialize ChatGPT integration if available
        chatGPTIntegration = GetComponent<ChatGPTIntegration>();
        
        // If we found the integration component, subscribe to its response event
        if (chatGPTIntegration != null)
        {
            chatGPTIntegration.OnResponseReceived += OnChatGPTResponseReceived;
            Debug.Log("Successfully connected to ChatGPTIntegration");
        }
        else
        {
            Debug.LogWarning("ChatGPTIntegration component not found. Using fallback messages.");
        }

        // Initialize audio source if not set
        if (typingSoundSource == null)
        {
            typingSoundSource = gameObject.AddComponent<AudioSource>();
            typingSoundSource.playOnAwake = false;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when this object is destroyed
        if (chatGPTIntegration != null)
        {
            chatGPTIntegration.OnResponseReceived -= OnChatGPTResponseReceived;
        }
    }

    private void Update()
    {
        // If boss is not active yet, don't proceed
        if (!IsBossActive())
            return;
            
        // State machine for message timing
        switch (currentState)
        {
            case DialogueState.Waiting:
                // Count up until it's time to show a message
                timer += Time.deltaTime;
                
                // Time to show a new message
                if (timer >= messageCooldownTime)
                {
                    if (messageQueue.Count == 0)
                    {
                        RequestNewMessage();
                    }
                    
                    // If we have a message to display
                    if (messageQueue.Count > 0)
                    {
                        string message = messageQueue.Dequeue();
                        DisplayMessage(message);
                        
                        // Switch to typing state if using typewriter effect
                        if (useTypewriterEffect)
                        {
                            currentState = DialogueState.Typing;
                        }
                        else
                        {
                            currentState = DialogueState.Displaying;
                        }
                        timer = 0f;
                    }
                }
                break;
                
            case DialogueState.Typing:
                // Handled by typewriter coroutine
                // The coroutine will change state to Displaying when done
                break;
                
            case DialogueState.Displaying:
                // Count time the message has been showing
                timer += Time.deltaTime;
                
                // Time to hide the message
                if (timer >= messageDisplayTime)
                {
                    HideMessage();
                }
                break;
        }
    }
    
    private bool IsBossActive()
    {
        // Check if the boss is active in the scene
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        return boss != null && boss.activeInHierarchy;
    }

    private void RequestNewMessage()
    {
        if (chatGPTIntegration != null && !chatGPTIntegration.IsProcessing())
        {
            Debug.Log("Requesting new message from ChatGPT");
            
            // Get the prompt using reflection since promptToSend is private
            string prompt = GetPromptFromChatGPT();
            if (!string.IsNullOrEmpty(prompt))
            {
                StartCoroutine(chatGPTIntegration.SendPromptToChatGPT(prompt));
            }
            else
            {
                // Use fallback if we can't get the prompt
                QueueRandomMessage();
            }
        }
        else
        {
            Debug.Log("Using fallback message (ChatGPT not available or busy)");
            QueueRandomMessage();
        }
    }
    
    private string GetPromptFromChatGPT()
    {
        try
        {
            // Access the private promptToSend field using reflection
            FieldInfo fieldInfo = chatGPTIntegration.GetType().GetField("promptToSend", 
                                      BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (fieldInfo != null)
            {
                string prompt = fieldInfo.GetValue(chatGPTIntegration) as string;
                if (!string.IsNullOrEmpty(prompt))
                {
                    Debug.Log("Successfully retrieved prompt from ChatGPTIntegration");
                    return prompt;
                }
            }
            
            // If we reach here, we couldn't get the prompt
            Debug.LogWarning("Could not access promptToSend field. Using default prompt.");
            return "Generate a short, threatening message from a boss in a video game, maximum 6 words.";
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error accessing prompt: {ex.Message}");
            return "Generate a short, threatening message from a boss in a video game, maximum 6 words.";
        }
    }
    
    private void QueueRandomMessage()
    {
        string randomMessage = GetRandomFallbackMessage();
        Debug.Log("Queued random message: " + randomMessage);
        messageQueue.Enqueue(randomMessage);
    }
    
    private string GetRandomFallbackMessage()
    {
        if (fallbackMessages == null || fallbackMessages.Length == 0)
            return "I will defeat you!";
            
        return fallbackMessages[UnityEngine.Random.Range(0, fallbackMessages.Length)];
    }
    
    private void OnChatGPTResponseReceived(string message)
    {
        Debug.Log("ChatGPT response received: " + message);
        // Add the message to the queue
        messageQueue.Enqueue(message);
    }
    
    private void DisplayMessage(string message)
    {
        if (dialogueBox == null || dialogueText == null)
        {
            Debug.LogError("Cannot display message: DialogueBox or DialogueText is null");
            return;
        }
        
        // Store the full message for typewriter effect
        currentFullMessage = message;
        
        // Play dialogue start sound
        if (dialogueStartSound != null && typingSoundSource != null)
        {
            typingSoundSource.PlayOneShot(dialogueStartSound, typingSoundVolume);
        }
            
        // Apply typewriter effect if enabled
        if (useTypewriterEffect)
        {
            // Start the typewriter effect first
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            typewriterCoroutine = StartCoroutine(TypewriterEffect(message));
            
            // Show the dialogue box only after starting the effect
            dialogueBox.SetActive(true);
        }
        else
        {
            // Show the dialogue box and set text immediately
            dialogueBox.SetActive(true);
            dialogueText.text = message;
            StartCoroutine(UpdateDialogueBoxSize());
        }
        
        Debug.Log("Displaying message: " + message);
        isMessageDisplaying = true;
    }

    private IEnumerator TypewriterEffect(string message)
    {
        // Wait a small delay before starting
        yield return new WaitForSeconds(0.2f);
        
        // Start with empty text
        dialogueText.text = "";
        
        // Reveal one character at a time
        for (int i = 0; i < message.Length; i++)
        {
            // Add the next character
            dialogueText.text = message.Substring(0, i + 1);
            
            // Update the box size as we type
            StartCoroutine(UpdateDialogueBoxSize());
            
            // Play typing sound with pitch variation
            if (typingSound != null && typingSoundSource != null)
            {
                typingSoundSource.pitch = UnityEngine.Random.Range(1f, typingSoundPitchVariation);
                typingSoundSource.PlayOneShot(typingSound, typingSoundVolume);
            }
            
            // Determine delay based on character
            float delay = typingSpeed;
            char currentChar = message[i];
            
            // Add extra delay after punctuation
            if (currentChar == '.' || currentChar == '!' || currentChar == '?' || currentChar == ',')
            {
                delay += delayAfterPunctuation;
            }
            
            // Wait before showing next character
            yield return new WaitForSeconds(delay);
        }
        
        // Typing finished, transition to displaying state
        currentState = DialogueState.Displaying;
        timer = 0f; // Reset timer for display duration
    }
    
    private IEnumerator UpdateDialogueBoxSize()
    {
        // Wait for end of frame to let the layout system process the text
        yield return new WaitForEndOfFrame();
        
        // Force layout update
        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueText.rectTransform);
        Canvas.ForceUpdateCanvases();
        
        if (backgroundRect != null)
        {
            // Get the size of the text
            Vector2 textSize = dialogueText.rectTransform.sizeDelta;
            
            // Apply padding and set the background size
            backgroundRect.sizeDelta = new Vector2(textSize.x + padding.x, textSize.y + padding.y);
        }
    }
    
    private void HideMessage()
    {
        // Stop any running typewriter effect
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
        
        // Simply hide the dialogue box
        dialogueBox.SetActive(false);
        isMessageDisplaying = false;
        
        // Switch to waiting state
        currentState = DialogueState.Waiting;
        timer = 0f;
        
        // Request next message in advance
        RequestNewMessage();
    }
    
    // Method to manually trigger a message display (can be called from other scripts or buttons)
    public void TriggerMessage()
    {
        Debug.Log("Message manually triggered");
        QueueRandomMessage();
        
        // Force immediate display if we're in waiting state
        if (currentState == DialogueState.Waiting)
        {
            timer = messageCooldownTime;
        }
    }
    
    // Method to skip the current typewriter animation
    public void SkipTypewriter()
    {
        if (currentState == DialogueState.Typing && !string.IsNullOrEmpty(currentFullMessage))
        {
            // Stop the typewriter coroutine
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }
            
            // Show the full message immediately
            dialogueText.text = currentFullMessage;
            
            // Update the box size
            StartCoroutine(UpdateDialogueBoxSize());
            
            // Switch to displaying state
            currentState = DialogueState.Displaying;
            timer = 0f;
        }
    }
}