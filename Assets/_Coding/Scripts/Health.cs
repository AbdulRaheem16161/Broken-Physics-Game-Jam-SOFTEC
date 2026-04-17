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

    #region 🔊 Audio (NEW)

    [Header("Damage Sounds")]
    [SerializeField] private List<AudioClip> damageClips = new List<AudioClip>();

    [Header("Death Sounds")]
    [SerializeField] private List<AudioClip> deathClips = new List<AudioClip>();

    [Header("Audio Settings")]
    [SerializeField] private float volume = 1f;
    [SerializeField] private Vector2 pitchVariation = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        #region Audio Setup

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // FULL 3D AUDIO
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 50f;
        audioSource.playOnAwake = false;

        #endregion
    }

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

    #endregion

    #region Damage System

    public void TakeDamage(float damage, GameObject attacker = null)
    {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, maxHp);

        if (damageFeedback != null)
            damageFeedback.PlayDamageFeedback();

        if (damageNumbers != null)
            damageNumbers.ShowDamage(damage);

        PlayRandomDamageSound(); // 🔊 NEW

        UpdateHealthUI();
        CheckLowHealth();

        if (hp <= 0f && !cantDie)
            HandleDeath();
    }

    #endregion

    #region Death System

    private void HandleDeath()
    {
        PlayRandomDeathSound(); // 🔊 NEW

        SpawnRandomObject();
        Destroy(gameObject, 0.05f); // tiny delay so sound actually plays
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

    #endregion

    #region 🔊 Audio Logic (NEW)

    private void PlayRandomDamageSound()
    {
        if (damageClips == null || damageClips.Count == 0 || audioSource == null)
            return;

        AudioClip clip = damageClips[Random.Range(0, damageClips.Count)];

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(clip, volume);
    }

    private void PlayRandomDeathSound()
    {
        if (deathClips == null || deathClips.Count == 0 || audioSource == null)
            return;

        AudioClip clip = deathClips[Random.Range(0, deathClips.Count)];

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(clip, volume * 1.2f); // slightly louder for death

    }

    #endregion

    #region Low Health Logic

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

    #region UI

    private void UpdateHealthUI()
    {
        if (healthBar == null) return;

        healthBar.fillAmount = hp / maxHp;

        float healthPercent = hp / maxHp;

        healthBar.color = (healthPercent <= lowHealthThreshold)
            ? lowHealthColor
            : normalColor;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = Color.red;

        Vector3 spawnPosition = transform.TransformPoint(spawnOffset);

        Gizmos.DrawSphere(spawnPosition, 0.3f);
        Gizmos.DrawLine(transform.position, spawnPosition);
    }

    #endregion
}