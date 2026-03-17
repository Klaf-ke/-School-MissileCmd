using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy groundEnemyPrefab;
    [SerializeField] private Enemy flyingEnemyPrefab;

    [SerializeField] private float flyingSpawnChance = 0.25f;
    [SerializeField] private TurretUIManager uiManager;
    [SerializeField] private TurretShooter[] turretShooters;

    [Header("Wave Settings")]
    [SerializeField] private float minSpawnDelay = 0.5f;
    [SerializeField] private float maxSpawnDelay = 2f;
    [SerializeField] private AudioClip waveCompleteSound;
    [SerializeField] private LooseScreenManager looseScreenManager;

    [SerializeField] private int baseEnemiesPerWave = 5;
    [SerializeField] private float difficultyMultiplier = 1.5f;
    
    [Header("Ammo Reward")]
    [SerializeField] private int ammoRewardPerWave = 20;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Boss Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float bossZoomDistance = 8f;
    [SerializeField] private float introDuration = 2f;  

   

    [Header("Bunker Settings")]
    [SerializeField] private Bunker[] bunkers;

    [Header("Spawn Points")]
    [SerializeField] private SpawnPoint[] spawnPoints;

    private bool waveInProgress = false;
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform spawnLocation;
        public Bunker linkedBunker;

        [HideInInspector]
        public bool isActive = true;
    }   

private int remainingBunkers;

    private int currentWave = 1;
    private int enemiesToSpawn;
    private int enemiesAlive;
    private int enemiesSpawned;

    private void Start()
    {
        remainingBunkers = bunkers.Length;

    
        foreach (var sp in spawnPoints)
        {
            sp.isActive = true;
        }

    
        foreach (Bunker bunker in bunkers)
        {
            bunker.OnBunkerDestroyed += HandleBunkerDestroyed;
        }

    StartWave();
}


    private void Update()
        {
    
        if (Input.GetKeyDown(KeyCode.K))
            {
                ClearWave();
            }

    
        if (Input.GetKeyDown(KeyCode.L))
            {
                SkipWave();
            }

        



    }   

    private void StartWave()
    {
        if (remainingBunkers <= 0) return; 

        Debug.Log("Starting Wave: " + currentWave);

        enemiesSpawned = 0;
        enemiesAlive = 0;
        waveInProgress = true;

        

        if (currentWave % 5 == 0)
        {
            enemiesToSpawn = 1;

            if (uiManager != null)
                uiManager.SetWave(currentWave, 1);

            SpawnBoss();
            return;
        }

         
        enemiesToSpawn = baseEnemiesPerWave + (currentWave * 4);

        if (uiManager != null)
            uiManager.SetWave(currentWave, enemiesToSpawn);

        UpgradeTurrets();

        StartCoroutine(SpawnWave());
    }

   private IEnumerator SpawnWave()
{
    while (enemiesSpawned < enemiesToSpawn)
    {
        SpawnEnemy();
        enemiesSpawned++;

        yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
    }

    while (enemiesAlive > 0)
        yield return null;

    WaveCompleted();
}

    private void WaveCompleted()
    {
        if (!waveInProgress) return;

        
        waveInProgress = false;

        if (waveCompleteSound != null)
            AudioSource.PlayClipAtPoint(waveCompleteSound, Vector3.zero);

        if (turretShooters != null)
        {
            foreach (var turret in turretShooters)
            {
                if (turret != null)
                turret.AddAmmo(ammoRewardPerWave + (currentWave * 2));
            }
        }

        currentWave++;
        Invoke(nameof(StartWave), 2f);
    }


    private void SpawnEnemy()
    {
        SpawnPoint chosenSpawn = GetRandomActiveSpawn();

        if (chosenSpawn == null)
        {
            Debug.Log("No active spawn points available.");
            return;
        }

        Enemy prefabToSpawn;

        if (Random.value < flyingSpawnChance)
            prefabToSpawn = flyingEnemyPrefab;
        else
            prefabToSpawn = groundEnemyPrefab;

        Enemy enemy = Instantiate(
            prefabToSpawn,
            chosenSpawn.spawnLocation.position,
            chosenSpawn.spawnLocation.rotation
        );

        enemiesAlive++; 

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetWaveManager(this);
            enemyScript.SetTargetBunker(chosenSpawn.linkedBunker);
        }
    }

        public void EnemyDied()
    {
        enemiesAlive--;

        if (uiManager != null)
            uiManager.EnemyKilled();

        if (waveInProgress && enemiesAlive <= 0 && enemiesSpawned >= enemiesToSpawn)
        {
            WaveCompleted();
        }
    }
    

private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
        Debug.LogWarning("Boss prefab not assigned!");
        return;
        }

        SpawnPoint chosenSpawn = GetRandomActiveSpawn();

        if (chosenSpawn == null)
        {
            Debug.Log("No active bunker for boss.");
            return;
        }

        GameObject boss = Instantiate(
            bossPrefab,
            bossSpawnPoint.position,
            bossSpawnPoint.rotation
        );

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossSpawn();
        }

    Enemy enemyScript = boss.GetComponent<Enemy>();

    if (enemyScript != null)
        {
            enemyScript.SetWaveManager(this);
            enemyScript.SetTargetBunker(chosenSpawn.linkedBunker);

           int healthSteps = currentWave / 5;
    float bonusHealth = healthSteps * 5f;

    enemyScript.AddBonusHealth(bonusHealth);
        }

        enemiesSpawned = 1;
        enemiesAlive = 1;

        StartCoroutine(BossIntroSequence(boss.transform));
    }

    private IEnumerator BossIntroSequence(Transform bossTransform)
    {
    
        Time.timeScale = 0.3f;

        Vector3 originalPosition = mainCamera.transform.position;
        Quaternion originalRotation = mainCamera.transform.rotation;

    
        Vector3 direction = (mainCamera.transform.position - bossTransform.position).normalized;
        Vector3 zoomPosition = bossTransform.position + direction * bossZoomDistance;

        float t = 0f;
        float duration = introDuration;

    
        while (t < duration)
        {
        t += Time.unscaledDeltaTime;
        float lerpValue = t / duration;

        mainCamera.transform.position =
            Vector3.Lerp(originalPosition, zoomPosition, lerpValue);

        mainCamera.transform.LookAt(bossTransform);

        yield return null;
        }

        yield return new WaitForSecondsRealtime(1f);

    
    t = 0f;
    Quaternion zoomRotation = mainCamera.transform.rotation;

while (t < duration)
    {
        t += Time.unscaledDeltaTime;
        float lerpValue = t / duration;

            mainCamera.transform.position =
        Vector3.Lerp(zoomPosition, originalPosition, lerpValue);

        mainCamera.transform.rotation =
            Quaternion.Lerp(zoomRotation, originalRotation, lerpValue);

        yield return null;
    }
        Time.timeScale = 1f;
    }

    private void ClearWave()
        {
            Debug.Log("Wave Cleared (Debug)");

            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

            foreach (Enemy enemy in enemies)
            {
                Destroy(enemy.gameObject);
            }

            enemiesAlive = 0;
        }

    private void SkipWave()
    {
        Debug.Log("Wave Skipped (Debug)");

        StopAllCoroutines();

        enemiesAlive = 0;
        enemiesSpawned = enemiesToSpawn;

        WaveCompleted();
    }




    private void HandleBunkerDestroyed(Bunker bunker)
    {
        remainingBunkers--;

    
        foreach (var sp in spawnPoints)
        {
            if (sp.linkedBunker == bunker)
            {
                sp.isActive = false;
                Debug.Log($"Spawn lane disabled for {bunker.name}");
            }
        }

        ClearWave();

        if (remainingBunkers <= 0)
        {
            GameOver();
        }
    }



  private void GameOver()
    {
    Debug.Log("GAME OVER - All bunkers destroyed!");

    
    StopAllCoroutines();

     if (looseScreenManager != null)
        looseScreenManager.ShowLooseScreen();

    Time.timeScale = 0f;
    }


    private SpawnPoint GetRandomActiveSpawn()
    {
        System.Collections.Generic.List<SpawnPoint> activeSpawns =
            new System.Collections.Generic.List<SpawnPoint>();

        foreach (var sp in spawnPoints)
        {
            if (sp.isActive &&
                sp.linkedBunker != null &&
                !sp.linkedBunker.IsDestroyed())
            {
                activeSpawns.Add(sp);
            }
        }

        if (activeSpawns.Count == 0)
            return null;

        return activeSpawns[Random.Range(0, activeSpawns.Count)];
    }

    private void UpgradeTurrets()
    {
        foreach (TurretShooter turret in turretShooters)
        {
            if (turret != null)
            {
                turret.UpgradeTurret(currentWave);
            }
        }
    }   

}