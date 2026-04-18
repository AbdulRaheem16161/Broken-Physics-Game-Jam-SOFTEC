using UnityEngine;

public class PowerUp_LargeEnemies : MonoBehaviour, IPowerUp
{

    [SerializeField] private string displayName = "Giant Enemies";

    public string DisplayName => displayName;
    #region References

    [Header("Spawner")]
    [SerializeField] private DifficultyManager enemySpawner;

    #endregion

    #region Settings

    [Header("PowerUp Settings")]
    [SerializeField] private float boostedSpawnsPerMinute = 25f;

    #endregion

    #region Runtime

    private float originalSpawnsPerMinute;

    #endregion

    private void Awake()
    {
        #region Auto Assign

        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<DifficultyManager>();
        }

        if (enemySpawner != null)
        {
            originalSpawnsPerMinute = enemySpawner.currentSpawnsPerMinute;
        }

        Debug.Log("[LargeEnemies] Spawner assigned: " + (enemySpawner != null));

        #endregion
    }

    #region IPowerUp

    public void ActivatePowerUp()
    {
        #region Enable Large Enemies + Increase Spawn Rate

        if (enemySpawner == null)
        {
            Debug.LogWarning("[LargeEnemies] EnemySpawner is NULL!");
            return;
        }

        enemySpawner.spawnLargeEnemies = true;
        enemySpawner.currentSpawnsPerMinute = boostedSpawnsPerMinute;

        Debug.Log("[LargeEnemies] ACTIVATED → Big enemies + faster spawns (" + boostedSpawnsPerMinute + "/min)");

        #endregion
    }

    public void DeactivatePowerUp()
    {
        #region Restore Normal State

        if (enemySpawner == null)
        {
            Debug.LogWarning("[LargeEnemies] EnemySpawner is NULL!");
            return;
        }

        enemySpawner.spawnLargeEnemies = false;
        enemySpawner.currentSpawnsPerMinute = originalSpawnsPerMinute;

        Debug.Log("[LargeEnemies] DEACTIVATED → Normal enemies + spawn rate restored");

        #endregion
    }

    #endregion
}