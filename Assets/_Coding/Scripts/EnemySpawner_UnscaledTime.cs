using UnityEngine;

public class EnemySpawnerTimeOverride : MonoBehaviour
{
    #region References
    [SerializeField] private EnemySpawner spawner;
    #endregion

    #region Runtime
    private float realTimer;
    #endregion

    private void Awake()
    {
        #region Auto Find Spawner

        if (spawner == null)
        {
            spawner = GetComponent<EnemySpawner>();
        }

        #endregion
    }

    private void Update()
    {
        #region Override Spawn Timing

        if (spawner == null) return;

        // Manually run spawn timer using unscaled time
        realTimer += Time.unscaledDeltaTime;

        float interval = 60f / Mathf.Max(spawner.spawnsPerMinute, 0.1f);

        if (realTimer >= interval)
        {
            realTimer = 0f;

            ForceSpawn();
        }

        #endregion
    }

    private void ForceSpawn()
    {
        #region Call Spawner Internals

        // We directly call spawn logic indirectly by reflection-free approach:
        // easiest clean method = just call public Spawn method via a small hack

        var method = typeof(EnemySpawner).GetMethod("SpawnEnemy",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            method.Invoke(spawner, null);
        }

        #endregion
    }
}