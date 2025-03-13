using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int spawnScore;
    
    [Header("Spawn References")]
    [SerializeField] private GameObject firstBall;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform bossSpawnPoint;

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
        }
    }
}
