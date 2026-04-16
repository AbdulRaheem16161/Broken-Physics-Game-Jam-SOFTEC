using System.Collections.Generic;
using UnityEngine;
using ArcadeVP;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableEnemy
    {
        public GameObject prefab;

        [Range(0f, 100f)]
        public float spawnChance = 50f;
    }

    #region Enemies
    [Header("Enemies")]
    public List<SpawnableEnemy> enemies = new List<SpawnableEnemy>();
    #endregion

    #region Spawn Settings
    [Header("Spawn Settings")]
    public float spawnsPerMinute = 10f;

    [Header("Spawn Limit")]
    public int maxAliveEnemies = 30;
    #endregion

    #region Spawn Points
    [Header("Spawn Points")]
    public List<Transform> spawnPoints = new List<Transform>();
    #endregion

    #region Gizmos
    [Header("Gizmos")]
    public bool showSpawnPointGizmos = true;
    public float gizmoRadius = 0.5f;
    #endregion

    #region Runtime
    private float timer;
    private Transform player;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    #endregion

    private void Start()
    {
        #region Find Player

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("EnemySpawner: Player not found! Make sure Player tag exists.");
        }

        #endregion
    }

    private void Update()
    {
        #region Update Loop

        CleanupDestroyedEnemies();
        HandleSpawning();

        #endregion
    }

    private void HandleSpawning()
    {
        #region Handle Spawning

        if (enemies.Count == 0) return;

        // Stop spawning if limit reached
        if (aliveEnemies.Count >= maxAliveEnemies) return;

        timer += Time.deltaTime;

        float interval = 60f / Mathf.Max(spawnsPerMinute, 0.1f);

        if (timer >= interval)
        {
            timer = 0f;
            SpawnEnemy();
        }

        #endregion
    }

    private void SpawnEnemy()
    {
        #region Spawn Enemy

        GameObject prefab = GetRandomEnemy();
        if (prefab == null) return;

        Vector3 spawnPos = GetRandomSpawnPoint();

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Track enemy
        aliveEnemies.Add(enemy);

        AssignTarget(enemy);

        #endregion
    }

    private void CleanupDestroyedEnemies()
    {
        #region Cleanup Destroyed

        aliveEnemies.RemoveAll(e => e == null);

        #endregion
    }

    private void AssignTarget(GameObject enemy)
    {
        #region Assign Target

        if (player == null) return;

        AICarBrain carAI = enemy.GetComponent<AICarBrain>();
        if (carAI != null)
        {
            carAI.target = player;
        }

        BouncyChaser bouncy = enemy.GetComponent<BouncyChaser>();
        if (bouncy != null)
        {
            bouncy.target = player;
        }

        #endregion
    }

    private GameObject GetRandomEnemy()
    {
        #region Weighted Random Enemy

        float totalWeight = 0f;

        foreach (var e in enemies)
        {
            if (e.prefab == null) continue;
            totalWeight += e.spawnChance;
        }

        float random = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var e in enemies)
        {
            if (e.prefab == null) continue;

            current += e.spawnChance;

            if (random <= current)
                return e.prefab;
        }

        return enemies.Count > 0 ? enemies[0].prefab : null;

        #endregion
    }

    private Vector3 GetRandomSpawnPoint()
    {
        #region Get Spawn Point

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: No spawn points assigned!");
            return transform.position;
        }

        int index = Random.Range(0, spawnPoints.Count);

        if (spawnPoints[index] == null)
        {
            Debug.LogWarning("EnemySpawner: Spawn point is null!");
            return transform.position;
        }

        return spawnPoints[index].position;

        #endregion
    }

    private void OnDrawGizmos()
    {
        #region Draw Spawn Points

        if (!showSpawnPointGizmos) return;
        if (spawnPoints == null) return;

        Gizmos.color = Color.green;

        foreach (var point in spawnPoints)
        {
            if (point != null)
            {
                // Center dot
                Gizmos.DrawSphere(point.position, gizmoRadius * 0.3f);

                // Outer circle
                Gizmos.DrawWireSphere(point.position, gizmoRadius);
            }
        }

        #endregion
    }
}