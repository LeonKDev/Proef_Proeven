using UnityEngine;

public class SparkEffectHandler : MonoBehaviour
{
    [Header("Spark Settings")]
    [SerializeField] private GameObject[] sparkPrefabs = new GameObject[3]; // Array of 3 different spark prefabs
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private int framesUntilDestroy = 3;
    
    private int frameCounter;
    private bool countingFrames;

    public void SpawnSpark(Vector3 position, Vector3 normal)
    {
        if (sparkPrefabs.Length == 0) return;

        // Randomly select a spark prefab
        int randomIndex = Random.Range(0, sparkPrefabs.Length);
        GameObject selectedPrefab = sparkPrefabs[randomIndex];

        if (selectedPrefab == null) return;

        // Create the spark at the collision point
        GameObject spark = Instantiate(selectedPrefab, position + positionOffset, Quaternion.identity);
        
        // Calculate rotation to face normal direction plus offset
        Quaternion rotation = Quaternion.LookRotation(normal);
        spark.transform.rotation = rotation * Quaternion.Euler(rotationOffset);
        
        // Start counting frames
        frameCounter = 0;
        countingFrames = true;
    }

    private void Update()
    {
        if (!countingFrames) return;

        frameCounter++;
        if (frameCounter >= framesUntilDestroy)
        {
            // Find and destroy all spark objects
            GameObject[] sparks = GameObject.FindGameObjectsWithTag("Spark");
            foreach (GameObject spark in sparks)
            {
                Destroy(spark);
            }
            countingFrames = false;
        }
    }
}