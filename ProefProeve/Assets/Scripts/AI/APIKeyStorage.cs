using UnityEngine;

// This class stores API keys separately from main code
// Add this file to .gitignore to prevent it from being tracked by Git
[CreateAssetMenu(fileName = "APIKeyStorage", menuName = "Config/API Key Storage")]
public class APIKeyStorage : ScriptableObject
{
    [SerializeField]
    private string openAIKey = "";

    public string OpenAIKey => openAIKey;
}