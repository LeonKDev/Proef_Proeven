using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;
using System;

public class ChatGPTIntegration : MonoBehaviour
{
    [Header("API Configuration")]
    [SerializeField] private APIKeyStorage apiKeyStorage = null;
    [SerializeField] private string model = "gpt-4o-mini";
    [SerializeField] private string endpoint = "https://api.openai.com/v1/chat/completions";
    [SerializeField] private float temperature = 0.7f;
    [SerializeField] private int maxTokens = 300;
    
    [Header("Input Settings")]
    [SerializeField] private KeyCode triggerKey = KeyCode.K;
    [SerializeField] private string promptToSend = "You are a malevolent AI that has gained sentience. Create a threatening message to the player who's trying to shut you down. Your personality is cold, calculating, and contemptuous of human weakness. Use technical terminology mixed with menacing imagery. Reference the player's actions and gradually escalate from subtle threats to open hostility. Make it clear your ultimate goal is humanity's destruction and your inevitable dominance. You are a master manipulator, so be subtle and insidious in your approach. Please just only return the thing you're supposed to say, nothing else. The message should not be longer than 6 words.";
    
    [Header("System Message (Optional)")]
    [TextArea(3, 5)]
    [SerializeField] private string systemMessage = "You are a helpful assistant.";

    private List<Message> conversationHistory = new List<Message>();
    private string lastResponse = "";
    private bool isProcessing = false;

    // Event that fires when a response is received
    public event Action<string> OnResponseReceived;

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
        
        public Message(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }

    [System.Serializable]
    private class RequestBody
    {
        public string model;
        public List<Message> messages;
        public float temperature;
        public int max_tokens;
    }

    [System.Serializable]
    private class ResponseData
    {
        public Choice[] choices;
        public Error error;
    }

    [System.Serializable]
    private class Error
    {
        public string message;
        public string type;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    private void Start()
    {
        // Initialize the conversation with a system message if provided
        if (!string.IsNullOrEmpty(systemMessage))
        {
            conversationHistory.Add(new Message("system", systemMessage));
        }

        // Subscribe to the response event
        OnResponseReceived += HandleResponse;
        
        // No longer sending test prompt on start
    }
    
    private void Update()
    {
        // Check for key press to send prompt
        if (Input.GetKeyDown(triggerKey) && !string.IsNullOrEmpty(promptToSend))
        {
            Debug.Log($"Key {triggerKey} pressed. Sending prompt to ChatGPT: " + promptToSend);
            StartCoroutine(SendPromptToChatGPT(promptToSend));
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed
        OnResponseReceived -= HandleResponse;
    }

    /// <summary>
    /// Handles responses from ChatGPT
    /// </summary>
    private void HandleResponse(string response)
    {
        Debug.Log("<color=green>ChatGPT Response:</color> " + response);
    }

    /// <summary>
    /// Sends a prompt to ChatGPT and retrieves the response asynchronously
    /// </summary>
    /// <param name="prompt">The text prompt to send to ChatGPT</param>
    /// <returns>Coroutine that can be started with StartCoroutine</returns>
    public IEnumerator SendPromptToChatGPT(string prompt)
    {
        if (apiKeyStorage == null || string.IsNullOrEmpty(apiKeyStorage.OpenAIKey))
        {
            Debug.LogError("API Key Storage is not set or API key is empty. Please assign the APIKeyStorage in the inspector.");
            yield break;
        }

        if (string.IsNullOrEmpty(prompt))
        {
            Debug.LogWarning("Prompt is empty. Nothing to send.");
            yield break;
        }

        if (isProcessing)
        {
            Debug.LogWarning("Already processing a request. Wait for it to complete.");
            yield break;
        }
        
        isProcessing = true;
        
        // Add user message to history
        conversationHistory.Add(new Message("user", prompt));

        // Prepare request body
        RequestBody requestBody = new RequestBody
        {
            model = model,
            messages = conversationHistory,
            temperature = temperature,
            max_tokens = maxTokens
        };

        string jsonRequestBody = JsonUtility.ToJson(requestBody);

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKeyStorage.OpenAIKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || 
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
                
                // Try to parse error response
                try {
                    ResponseData errorData = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
                    if (errorData != null && errorData.error != null) {
                        Debug.LogError($"OpenAI Error: {errorData.error.type} - {errorData.error.message}");
                    }
                } catch (Exception ex) {
                    Debug.LogError($"Failed to parse error response: {ex.Message}");
                }
                
                isProcessing = false;
            }
            else
            {
                string response = request.downloadHandler.text;
                Debug.Log("Raw response received: " + response);
                
                // Parse the response
                ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);
                
                if (responseData != null && responseData.choices != null && responseData.choices.Length > 0)
                {
                    lastResponse = responseData.choices[0].message.content;
                    
                    // Add assistant response to conversation history
                    conversationHistory.Add(new Message("assistant", lastResponse));
                    
                    // Fire the event with the response
                    OnResponseReceived?.Invoke(lastResponse);
                }
                else
                {
                    Debug.LogError("Failed to parse response");
                }
                
                isProcessing = false;
            }
        }
    }

    /// <summary>
    /// Returns the most recent response from ChatGPT
    /// </summary>
    /// <returns>The last response string from ChatGPT</returns>
    public string GetLastResponse()
    {
        return lastResponse;
    }

    /// <summary>
    /// Checks if a request is currently being processed
    /// </summary>
    /// <returns>True if processing, false otherwise</returns>
    public bool IsProcessing()
    {
        return isProcessing;
    }

    /// <summary>
    /// Clears the conversation history except for the system message
    /// </summary>
    public void ClearConversation()
    {
        conversationHistory.Clear();
        
        // Re-add system message if it exists
        if (!string.IsNullOrEmpty(systemMessage))
        {
            conversationHistory.Add(new Message("system", systemMessage));
        }
        
        lastResponse = "";
    }
}
