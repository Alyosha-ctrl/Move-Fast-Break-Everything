using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    private struct SpawnPhase
    {
        public float startTimeSeconds;
        public int maxEnemies;
        public float spawnInterval;
    }
    
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform player;
    [SerializeField] private float spawnDistance = 18f; // With current setup, 18f is just outside camera view
    [SerializeField] private float respawnDistance = 40f;
    [SerializeField] private SpawnPhase[] spawnPhases;

    [SerializeField] private GameObject boss1Prefab;
    [SerializeField] private GameObject boss2Prefab;
    [SerializeField] private GameObject boss3Prefab;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float boss1SpawnTime = 120f;
    [SerializeField] private float boss2SpawnTime = 600f;
    [SerializeField] private float boss3SpawnTime = 900f;
    
    private bool bossSpawned = false;
    private bool bossActive = false;
    
    private readonly List<GameObject> activeEnemies = new();
    private float spawnTimer;

    private void Awake()
    {
        StartCoroutine(SpawnBossTimed(boss1SpawnTime, boss1Prefab));
        StartCoroutine(SpawnBossTimed(boss2SpawnTime, boss2Prefab));
        StartCoroutine(SpawnBossTimed(boss3SpawnTime, boss3Prefab));
    }
    
    private void Update()
    {
        // if (!bossSpawned && GameManager.CurrentRunTimeSeconds >= boss3SpawnTime)
        // {
        //     SpawnBoss();
        //     return;
        // }
        
        

        if (bossActive) return;

        if (player == null)
        {
            return;
        }

        UpdateActiveEnemies();
        GetSpawnPhase(out int currentMaxEnemies, out float currentSpawnInterval);
        
        if (activeEnemies.Count >= currentMaxEnemies)
        {
            return;
        }
        
        spawnTimer -= Time.deltaTime;
        
        if (spawnTimer > 0f)
        {
            return;
        }
        
        SpawnEnemy();
        spawnTimer = currentSpawnInterval;
        
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned!");
            return;
        }
        Vector3 spawnPosition = GetSpawn();

        
        GameObject randEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        GameObject enemy = Instantiate(randEnemy, spawnPosition, Quaternion.identity, transform);
        activeEnemies.Add(enemy);
    }

    private System.Collections.IEnumerator SpawnBossTimed(float time, GameObject bossPrefab)
    {
        yield return new WaitForSeconds(time);
        SpawnBoss(bossPrefab);

    }

    private void SpawnBoss(GameObject bossPrefab)
    {
        bossSpawned = true;
        bossActive = true;

        Vector3 spawnPos = player.position + Vector3.up * 3f;
        GameObject bossObject = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
        BossController boss = bossObject.GetComponent<BossController>();
        boss.Initialize(this, player, virtualCamera);
    }

    public void OnBossDied()
    {
        bossActive = false;
        //Spawn a shit ton of exp.
    }
    
    // Picks random position around player at a set distance
    private Vector3 GetSpawn()
    {
        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + new Vector3(spawnDirection.x, spawnDirection.y, 0f) * spawnDistance;
        spawnPosition.z = 0f;
        return spawnPosition;
    }
    
    // Removes destroyed enemy references and moves distant enemies back near the player
    private void UpdateActiveEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            GameObject enemyObject = activeEnemies[i];

            if (enemyObject == null)
            {
                activeEnemies.RemoveAt(i);
                continue;
            }

            if (Vector2.Distance(enemyObject.transform.position, player.position) > respawnDistance)
            {
                enemyObject.transform.position = GetSpawn();
            }
        }
    }

    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
    }
    
    // Picks the active spawn phase based on current run time
    private void GetSpawnPhase(out int currentMaxEnemies, out float currentSpawnInterval)
    {
        // Defaults to 0 if no phases are added
        if (spawnPhases == null || spawnPhases.Length == 0)
        {
            currentMaxEnemies = 0;
            currentSpawnInterval = 0f;
            return;
        }
        
        float runTimeSeconds = GameManager.CurrentRunTimeSeconds;
        
        for (int i = 0; i < spawnPhases.Length - 1; i++)
        {
            SpawnPhase currentPhase = spawnPhases[i];
            SpawnPhase nextPhase = spawnPhases[i + 1];
            
            if (runTimeSeconds < nextPhase.startTimeSeconds)
            {
                float percentToNextPhase = Mathf.InverseLerp(
                    currentPhase.startTimeSeconds,
                    nextPhase.startTimeSeconds,
                    runTimeSeconds
                );
                
                // Gradually increases from current phase values to next phase values
                currentMaxEnemies = Mathf.RoundToInt(Mathf.Lerp(currentPhase.maxEnemies, nextPhase.maxEnemies, percentToNextPhase));
                currentSpawnInterval = Mathf.Lerp(currentPhase.spawnInterval, nextPhase.spawnInterval, percentToNextPhase);
                return;
            }
        }
        
        SpawnPhase lastPhase = spawnPhases[spawnPhases.Length - 1];
        currentMaxEnemies = lastPhase.maxEnemies;
        currentSpawnInterval = lastPhase.spawnInterval;
    }
}
