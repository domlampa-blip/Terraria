using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] enemyPrefabs; // Changed to array for variety
    public float spawnRate = 5f;
    public float spawnRateDecrease = 0.05f; // Gets faster over time
    public float minSpawnRate = 1f;

    [Header("Area")]
    public int worldWidth = 100; // Match WorldGenerator
    public int spawnHeightCheck = 50; // Height to raycast from
    public LayerMask groundLayer;

    private float nextSpawnTime;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            
            // Difficulty curve
            spawnRate = Mathf.Max(minSpawnRate, spawnRate - spawnRateDecrease);
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnEnemy()
    {
        // Pick random X
        int randomX = Random.Range(1, worldWidth - 1);
        
        // Find ground height
        Vector2 rayOrigin = new Vector2(randomX, spawnHeightCheck);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 100f, groundLayer);

        if (hit.collider != null)
        {
            Vector3 spawnPos = hit.point;
            spawnPos.y += 0.5f; // Slight offset up

            if (enemyPrefabs != null && enemyPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(0, enemyPrefabs.Length);
                Instantiate(enemyPrefabs[randomIndex], spawnPos, Quaternion.identity);
            }
        }
    }
}
