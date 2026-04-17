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

    [Header("Low Health UI")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.25f;

    #endregion

    #region Low Health Effect

    [Header("Low Health Effect")]
    [SerializeField] private GameObject lowHealthEffectPrefab;

    private GameObject spawnedLowHealthEffect;
    private bool lowHealthTriggered;

    #endregion

    #region Feedback

    [SerializeField] private DamageFeedback damageFeedback;
    [SerializeField] private DamageNumbers damageNumbers;

    #endregion

    #region Death Spawn

    [Header("Death Spawn")]
    [SerializeField] private List<GameObject> deathPrefabs = new List<GameObject>();
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
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
        #region Debug Kill
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
        CheckLowHealth();

        if (hp <= 0f && !cantDie)
            HandleDeath();
    }

    private void HandleDeath()
    {
        SpawnRandomObject();
        Destroy(gameObject);
    }

    private void SpawnRandomObject()
    {
        if (deathPrefabs == null || deathPrefabs.Count == 0)
            return;

        int randomIndex = Random.Range(0, deathPrefabs.Count);
        GameObject prefabToSpawn = deathPrefabs[randomIndex];

        Vector3 spawnPosition = transform.TransformPoint(spawnOffset);

        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    #region LOW HEALTH LOGIC

    private void CheckLowHealth()
    {
        float healthPercent = hp / maxHp;

        if (healthPercent <= lowHealthThreshold && !lowHealthTriggered)
        {
            lowHealthTriggered = true;
            ActivateLowHealthEffect();
        }
    }

    private void ActivateLowHealthEffect()
    {
        if (lowHealthEffectPrefab == null) return;

        spawnedLowHealthEffect = Instantiate(lowHealthEffectPrefab);

        spawnedLowHealthEffect.transform.SetParent(transform);
        spawnedLowHealthEffect.transform.localPosition = Vector3.zero;
        spawnedLowHealthEffect.transform.localRotation = Quaternion.identity;
    }

    #endregion

    private void UpdateHealthUI()
    {
        if (healthBar == null) return;

        healthBar.fillAmount = hp / maxHp;

        float healthPercent = hp / maxHp;

        healthBar.color = (healthPercent <= lowHealthThreshold)
            ? lowHealthColor
            : normalColor;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.red;

        Vector3 spawnPosition = transform.TransformPoint(spawnOffset);

        Gizmos.DrawSphere(spawnPosition, 0.3f);
        Gizmos.DrawLine(transform.position, spawnPosition);
    }
}