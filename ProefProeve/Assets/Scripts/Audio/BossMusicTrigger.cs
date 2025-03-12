using UnityEngine;

/// <summary>
/// Attach this component to a boss or trigger zone to manually start boss music
/// </summary>
public class BossMusicTrigger : MonoBehaviour
{
    [SerializeField] private bool triggerOnStart = false;
    [SerializeField] private bool triggerOnTriggerEnter = true;
    [SerializeField] private string playerTag = "Player";
    
    private void Start()
    {
        if (triggerOnStart)
        {
            TriggerBossMusic();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnTriggerEnter && other.CompareTag(playerTag))
        {
            TriggerBossMusic();
        }
    }
    
    /// <summary>
    /// Manually trigger the boss music
    /// </summary>
    public void TriggerBossMusic()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.TriggerBossMusicManually();
        }
        else
        {
            Debug.LogWarning("MusicManager not found in scene. Can't trigger boss music.");
        }
    }
}