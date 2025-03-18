using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple scene transition handler with music integration
/// Attach to a GameObject in your starting scene
/// </summary>
public class SceneTransitionHandler : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "MainMenu";
    [SerializeField] private string gameSceneName = "GameScene";
    
    /// <summary>
    /// Transitions to the menu scene and plays menu music
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayMenuMusic();
        }
    }
    
    /// <summary>
    /// Transitions to the game scene and plays gameplay music
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayGameplayMusic();
        }
    }
    
    /// <summary>
    /// Can be used with UI buttons to manually trigger music changes
    /// </summary>
    public void TriggerBossMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.TriggerBossMusicManually();
        }
    }
    
    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}