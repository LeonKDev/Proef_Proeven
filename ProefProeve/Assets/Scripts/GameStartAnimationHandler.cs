using UnityEngine;

/// <summary>
/// Handles animation events for the game start animation
/// This component should be attached to the GameObject containing the Animator
/// </summary>
public class GameStartAnimationHandler : MonoBehaviour
{
    [Tooltip("Reference to the animator playing the start animation")]
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

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

    /// <summary>
    /// A helper method to preview the animation in the editor
    /// </summary>
    /// <param name="animationTrigger">The name of the animation trigger to activate</param>
    public void PlayAnimation(string animationTrigger)
    {
        if (animator != null && !string.IsNullOrEmpty(animationTrigger))
        {
            animator.SetTrigger(animationTrigger);
        }
    }
}