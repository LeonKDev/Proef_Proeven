using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkEffectHandler : MonoBehaviour
{
    [Header("Spark Settings")]
    [SerializeField] private GameObject[] sparkPrefabs = new GameObject[3]; // Array of 3 different spark prefabs
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private float destroyDelay = 1.0f; // Time in seconds to destroy spark effects
    
    // List to track active spark objects
    private List<GameObject> activeSparkObjects = new List<GameObject>();
    
    private void Awake()
    {
        // Validate prefabs on startup
        ValidatePrefabs();
    }
    
    private void ValidatePrefabs()
    {
        bool prefabMissing = false;
        for (int i = 0; i < sparkPrefabs.Length; i++)
        {
            if (sparkPrefabs[i] == null)
            {
                Debug.LogWarning("SparkEffectHandler: Spark prefab at index " + i + " is not assigned!");
                prefabMissing = true;
            }
        }
        
        if (prefabMissing)
        {
            Debug.LogError("SparkEffectHandler: Some spark prefabs are missing. Spark effects may not work properly.");
        }
    }

    public void SpawnSpark(Vector3 position, Vector3 normal)
    {
        if (sparkPrefabs.Length == 0)
        {
            Debug.LogWarning("SparkEffectHandler: No spark prefabs assigned!");
            return;
        }

        // Randomly select a spark prefab
        int randomIndex = Random.Range(0, sparkPrefabs.Length);
        GameObject selectedPrefab = sparkPrefabs[randomIndex];

        if (selectedPrefab == null)
        {
            Debug.LogWarning("SparkEffectHandler: Selected spark prefab is null!");
            return;
        }

        // Create the spark at the collision point
        GameObject spark = Instantiate(selectedPrefab, position + positionOffset, Quaternion.identity);
        
        // Calculate rotation to face normal direction plus offset
        Quaternion rotation = Quaternion.LookRotation(normal);
        spark.transform.rotation = rotation * Quaternion.Euler(rotationOffset);
        
        // Use destroy delay instead of frame counting
        // This ensures the effect works even when Time.timeScale is modified
        Destroy(spark, destroyDelay);
        
        // Add to tracking list
        activeSparkObjects.Add(spark);
        
        // Remove null entries from the tracking list
        CleanupList();
    }
    
    private void CleanupList()
    {
        // Remove any destroyed objects from the tracking list
        for (int i = activeSparkObjects.Count - 1; i >= 0; i--)
        {
            if (activeSparkObjects[i] == null)
            {
                activeSparkObjects.RemoveAt(i);
            }
        }
    }
    
    // Public method to destroy all active sparks immediately
    public void ClearAllSparks()
    {
        foreach (GameObject spark in activeSparkObjects)
        {
            if (spark != null)
            {
                Destroy(spark);
            }
        }
        activeSparkObjects.Clear();
    }
}