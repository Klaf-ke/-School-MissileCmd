using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private SkySpawner[] spawners;
    [SerializeField] private TurretUIManager uiManager;

    [Header("Wave Settings")]
    [SerializeField] private float minSpawnDelay = 0.5f;
    [SerializeField] private float maxSpawnDelay = 2f;

    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float difficultyMultiplier = 1.5f;

    private int currentWave = 1;
    private int enemiesToSpawn;
    private int enemiesAlive;
    private int enemiesSpawned;

    private void Start()
    {
        StartWave();
    }

    private void StartWave()
    {
        Debug.Log("Starting Wave: " + currentWave);

        enemiesToSpawn = Mathf.RoundToInt(
            baseEnemiesPerWave * Mathf.Pow(difficultyMultiplier, currentWave - 1)
        );

        enemiesSpawned = 0;
        enemiesAlive = 0;

        // ✅ Tell UI a new wave started
        if (uiManager != null)
            uiManager.SetWave(currentWave, enemiesToSpawn);

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        while (enemiesSpawned < enemiesToSpawn)
        {
            SpawnEnemy();
            enemiesSpawned++;

            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private void SpawnEnemy()
    {
        int randomSpawnerIndex = Random.Range(0, spawners.Length);
        Vector3 spawnPosition = spawners[randomSpawnerIndex].transform.position;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        enemiesAlive++;

        // Give enemy reference to this manager
        enemy.GetComponent<Enemy>().SetWaveManager(this);
    }

    public void EnemyDied()
    {
        enemiesAlive--;

        // ✅ Update UI when enemy dies
        if (uiManager != null)
            uiManager.EnemyKilled();

        if (enemiesAlive <= 0 && enemiesSpawned >= enemiesToSpawn)
        {
            currentWave++;
            Invoke(nameof(StartWave), 2f);
        }
    }
}
