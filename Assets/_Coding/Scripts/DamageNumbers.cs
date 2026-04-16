using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    #region Settings

    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [Header("Spam Control")]
    [SerializeField] private float spawnCooldown = 0.08f;

    #endregion

    #region Runtime

    private float lastSpawnTime;

    #endregion

    #region Public Method

    public void ShowDamage(float damage)
    {
        #region Cooldown Check

        if (Time.time < lastSpawnTime + spawnCooldown)
        {
            return; // ignore spam hits
        }

        lastSpawnTime = Time.time;

        #endregion

        #region Safety Check

        if (damageNumberPrefab == null) return;

        #endregion

        #region Position

        Vector3 spawnPos = spawnPoint != null
            ? spawnPoint.position
            : transform.position + offset;

        #endregion

        #region Spawn

        GameObject obj = Instantiate(damageNumberPrefab, spawnPos, Quaternion.identity);

        DamageNumber dn = obj.GetComponent<DamageNumber>();

        if (dn != null)
        {
            dn.SetDamage(damage);
        }

        #endregion
    }

    #endregion
}