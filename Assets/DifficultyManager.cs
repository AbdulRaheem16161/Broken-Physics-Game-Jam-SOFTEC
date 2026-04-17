// DifficultyManager.cs
using System.Collections.Generic;
using UnityEngine;
using ArcadeVP;

public class DifficultyManager : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableEnemy
    {
        public GameObject prefab;

        [Header("Large Variant")]
        public GameObject largePrefab;

        [Range(0f, 100f)]
        public float spawnChance = 50f;
    }

    #region Enemies
    [Header("Enemies")]
    public List<SpawnableEnemy> enemies = new List<SpawnableEnemy>();
    #endregion

    #region Spawn Settings
    [Header("Spawn Settings")]
    public float baseSpawnsPerMinute = 5f;

    [Header("Spawn Limit")]
    public int maxAliveEnemies = 30;

    [Header("Minimum Difficulty Floor")]
    [SerializeField] private float minSpawnsPerMinute = 2f;

    [Header("Variants")]
    public bool spawnLargeEnemies = false;
    #endregion

    #region Difficulty Formula
    [Header("Difficulty Formula")]
    [SerializeField] private float difficultyMultiplier = 1.5f;

    [SerializeField] private float maxSpawnsPerMinute = 60f;
    #endregion

    #region Live Debug Values
    [Header("Live Debug Values")]
    [SerializeField] private int totalKills;
    [SerializeField] private float killsPerMinute;
    [SerializeField] public float currentSpawnsPerMinute;
    [SerializeField] private float spawnIntervalSeconds;
    [SerializeField] private int aliveEnemyCount;
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

    #region References
    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;
    #endregion

    #region Runtime
    private float timer;
    private Transform player;
    private List<GameObject> aliveEnemies = new List<GameObject>();
    #endregion

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    private void Update()
    {
        CleanupDestroyedEnemies();
        UpdateLiveDebugValues();
        HandleSpawning();
    }

    private void UpdateLiveDebugValues()
    {
        if (scoreManager != null)
        {
            totalKills = TryGetInt(scoreManager, "totalKills") ??
                         TryGetInt(scoreManager, "kills") ??
                         TryGetInt(scoreManager, "killCount") ??
                         0;

            killsPerMinute = scoreManager.GetKillsPerMinute();
        }
        else
        {
            totalKills = 0;
            killsPerMinute = 0f;
        }

        currentSpawnsPerMinute = ComputeSpawnsPerMinute();
        spawnIntervalSeconds = 60f / Mathf.Max(currentSpawnsPerMinute, 0.1f);
        aliveEnemyCount = aliveEnemies.Count;
    }

    private float ComputeSpawnsPerMinute()
    {
        float kpm = killsPerMinute;

        float computed = kpm > 0f
            ? kpm * difficultyMultiplier
            : baseSpawnsPerMinute;

        // 🔥 KEY CHANGE: minimum floor applied here
        computed = Mathf.Max(computed, minSpawnsPerMinute);

        return Mathf.Clamp(computed, minSpawnsPerMinute, maxSpawnsPerMinute);
    }

    private void HandleSpawning()
    {
        if (enemies.Count == 0) return;
        if (aliveEnemies.Count >= maxAliveEnemies) return;

        timer += Time.deltaTime;

        if (timer >= spawnIntervalSeconds)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        SpawnableEnemy selected = GetRandomEnemyData();
        if (selected == null) return;

        GameObject prefab = GetCorrectPrefab(selected);
        if (prefab == null) return;

        Vector3 pos = GetRandomSpawnPoint();
        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);

        aliveEnemies.Add(enemy);
        AssignTarget(enemy);
    }

    private GameObject GetCorrectPrefab(SpawnableEnemy enemy)
    {
        if (spawnLargeEnemies && enemy.largePrefab != null)
            return enemy.largePrefab;

        return enemy.prefab;
    }

    private void CleanupDestroyedEnemies()
    {
        aliveEnemies.RemoveAll(e => e == null);
    }

    private void AssignTarget(GameObject enemy)
    {
        if (player == null) return;

        AICarBrain carAI = enemy.GetComponent<AICarBrain>();
        if (carAI != null) carAI.target = player;

        BouncyChaser bouncy = enemy.GetComponent<BouncyChaser>();
        if (bouncy != null) bouncy.target = player;
    }

    private SpawnableEnemy GetRandomEnemyData()
    {
        float total = 0f;

        foreach (var e in enemies)
            if (e.prefab != null) total += e.spawnChance;

        float rand = Random.Range(0f, total);
        float current = 0f;

        foreach (var e in enemies)
        {
            if (e.prefab == null) continue;

            current += e.spawnChance;
            if (rand <= current)
                return e;
        }

        return enemies.Count > 0 ? enemies[0] : null;
    }

    private Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints.Count == 0)
            return transform.position;

        Transform t = spawnPoints[Random.Range(0, spawnPoints.Count)];
        return t != null ? t.position : transform.position;
    }

    private void OnDrawGizmos()
    {
        if (!showSpawnPointGizmos) return;

        Gizmos.color = Color.red;

        foreach (var p in spawnPoints)
        {
            if (p == null) continue;

            Gizmos.DrawSphere(p.position, gizmoRadius * 0.3f);
            Gizmos.DrawWireSphere(p.position, gizmoRadius);
        }
    }

    #region Reflection Helper
    private int? TryGetInt(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName);
        if (field != null && field.FieldType == typeof(int))
            return (int)field.GetValue(obj);

        var prop = obj.GetType().GetProperty(fieldName);
        if (prop != null && prop.PropertyType == typeof(int))
            return (int)prop.GetValue(obj);

        return null;
    }
    #endregion
}