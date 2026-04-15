using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableEnemy
    {
        public GameObject prefab;
        [Range(0f, 100f)]
        public float spawnChance = 50f;
    }

    [Header("Enemies")]
    public List<SpawnableEnemy> enemies = new List<SpawnableEnemy>();

    [Header("Spawn Settings")]
    public float spawnsPerMinute = 10f;

    [Header("Spawn Area")]
    public float width = 10f;
    public float height = 10f;

    [Header("Optional")]
    public Transform spawnCenter;

    private float timer;

    private void Update()
    {
        HandleSpawning();
    }

    private void HandleSpawning()
    {
        if (enemies.Count == 0) return;

        timer += Time.deltaTime;

        float interval = 60f / Mathf.Max(spawnsPerMinute, 0.1f);

        if (timer >= interval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject selected = GetRandomEnemy();

        if (selected == null) return;

        Vector3 spawnPos = GetRandomPosition();
        Instantiate(selected, spawnPos, Quaternion.identity);
    }

    private GameObject GetRandomEnemy()
    {
        float totalWeight = 0f;

        foreach (var e in enemies)
        {
            totalWeight += e.spawnChance;
        }

        float random = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var e in enemies)
        {
            current += e.spawnChance;

            if (random <= current)
            {
                return e.prefab;
            }
        }

        return enemies[0].prefab;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 center = spawnCenter != null ? spawnCenter.position : transform.position;

        float x = Random.Range(-width / 2f, width / 2f);
        float z = Random.Range(-height / 2f, height / 2f);

        return new Vector3(center.x + x, center.y, center.z + z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 center = spawnCenter != null ? spawnCenter.position : transform.position;

        Gizmos.DrawWireCube(center, new Vector3(width, 0.1f, height));
    }
}