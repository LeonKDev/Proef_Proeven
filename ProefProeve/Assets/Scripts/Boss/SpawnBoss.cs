using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int spawnScore;
    
    [Header("Spawn References")]
    [SerializeField] private GameObject firstBall;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform bossSpawnPoint;
    
    [Header("Dialogue System")]
    [SerializeField] private GameObject bossDialogueSystem;

    private bool bossSpawned;

    private void Update()
    {
        // Spawns the boss if the desired score is reached
        if (ScoreManager.Instance.Score >= spawnScore && !bossSpawned)
        {
            bossSpawned = true;
            Time.timeScale = 1;
            Destroy(firstBall);
            Time.timeScale = 1;
            boss.gameObject.transform.position = bossSpawnPoint.transform.position;
            boss.SetActive(true);
            
            // Play boss theme music when the boss is spawned
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.TriggerBossMusicManually();
            }
            else
            {
                Debug.LogWarning("MusicManager not found in scene. Can't play boss music.");
            }
            
            // Initialize the boss dialogue system
            if (bossDialogueSystem != null)
            {
                bossDialogueSystem.SetActive(true);
            }
        }
    }
}
