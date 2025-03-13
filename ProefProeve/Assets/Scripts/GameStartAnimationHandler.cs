using UnityEngine;

/// <summary>
/// Handles animation events for the game start animation
/// This component should be attached to the GameObject containing the Animator
/// </summary>
public class GameStartAnimationHandler : MonoBehaviour
{
    /// <summary>
    /// This method should be called as an Animation Event at the end of your start animation
    /// </summary>
    public void OnAnimationComplete()
    {
        // Notify the GameManager that the animation has completed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AnimationCompleted();
        }
    }
}