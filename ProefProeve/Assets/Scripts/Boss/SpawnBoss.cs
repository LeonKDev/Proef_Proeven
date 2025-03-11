using System.Security.Cryptography;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [SerializeField] private int SpawnAmount;
    [SerializeField] private GameObject firstBall;
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    private bool bossSpawned;
    
    // Update is called once per frame
    void Update()
    {
        if (ScoreManager.Instance.Score >= SpawnAmount && !bossSpawned)
        {
            bossSpawned = true;
            Time.timeScale = 1;
            Destroy(firstBall);
            Instantiate(bossPrefab, bossSpawnPoint);
        }
    }
}
