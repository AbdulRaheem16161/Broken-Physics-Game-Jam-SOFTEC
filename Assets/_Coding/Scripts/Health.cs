using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Health : MonoBehaviour
{
    #region Health

    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float hp = 100f;
    [SerializeField] private bool cantDie = false;

    #endregion

    #region UI

    [SerializeField] private Image healthBar;

    #endregion

    #region Feedback

    [SerializeField] private DamageFeedback damageFeedback;
    [SerializeField] private DamageNumbers damageNumbers;

    #endregion

    #region Death Spawn

    [Header("Death Spawn")]

    // List of objects to spawn randomly
    [SerializeField] private List<GameObject> deathPrefabs = new List<GameObject>();

    // Local offset (relative to player)
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;

    // Toggle gizmos
    [SerializeField] private bool showGizmos = true;

    #endregion


    private void Start()
    {
        if (hp <= 0f)
            hp = maxHp;

        if (damageFeedback == null)
            damageFeedback = GetComponent<DamageFeedback>();

        if (damageNumbers == null)
            damageNumbers = GetComponent<DamageNumbers>();

        UpdateHealthUI();
    }


    private void Update()
    {
        #region Debug Kill (Optional)
        // Press K to test death instantly (remove if you’re emotionally stable)
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(maxHp);
        }
        #endregion
    }


    public void TakeDamage(float damage, GameObject attacker = null)
    {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, maxHp);

        if (damageFeedback != null)
            damageFeedback.PlayDamageFeedback();

        if (damageNumbers != null)
            damageNumbers.ShowDamage(damage);

        UpdateHealthUI();

        if (hp <= 0f && !cantDie)
            HandleDeath();
    }


    private void HandleDeath()
    {
        #region Handle Death

        SpawnRandomObject();

        Destroy(gameObject);

        #endregion
    }


    private void SpawnRandomObject()
    {
        #region Spawn Random Object

        // Safety check
        if (deathPrefabs == null || deathPrefabs.Count == 0)
            return;

        // Pick random prefab
        int randomIndex = Random.Range(0, deathPrefabs.Count);
        GameObject prefabToSpawn = deathPrefabs[randomIndex];

        // Convert offset from local to world space
        Vector3 spawnPosition = transform.TransformPoint(spawnOffset);

        // Spawn it
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        #endregion
    }


    private void UpdateHealthUI()
    {
        if (healthBar == null) return;
        healthBar.fillAmount = hp / maxHp;
    }


    private void OnDrawGizmos()
    {
        #region Draw Gizmos

        if (!showGizmos)
            return;

        Gizmos.color = Color.red;

        // Draw sphere at spawn point
        Vector3 spawnPosition = transform.TransformPoint(spawnOffset);
        Gizmos.DrawSphere(spawnPosition, 0.3f);

        // Draw line from player to spawn point
        Gizmos.DrawLine(transform.position, spawnPosition);

        #endregion
    }
}