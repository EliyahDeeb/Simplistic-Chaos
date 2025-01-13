using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Enemy to spawn
    public Transform topBorder;    // Top part of the box where enemies spawn
    public float spawnInterval = 2f; // Time between spawns
    public float spawnRangeX = 5f;  // Horizontal spawn range along the top border

    private float nextSpawnTime;

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        // Get a random x position along the top border
        float spawnX = Random.Range(-spawnRangeX, spawnRangeX);
        
        Vector3 spawnPosition = new Vector3(topBorder.position.x + Random.Range(-spawnRangeX, spawnRangeX), topBorder.position.y, 0f);
        
        // Spawn enemy
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
