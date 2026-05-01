using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class BossSpawnData
{
    public string bossName;
    public int spawnLevel;
    public GameObject bossPrefab;
    public Transform spawnPoint;

    [HideInInspector] public bool hasSpawned = false;
}

public class ExpManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Level System")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float baseExpRequired = 100f;
    [SerializeField] private float expMultiplier = 1.25f;

    [Header("Boss System")]
    [SerializeField] private List<BossSpawnData> bosses = new List<BossSpawnData>();

    [Header("Level Up Effect")]
    [SerializeField] private GameObject levelUpEffectPrefab;
    [SerializeField] private float levelUpEffectDuration = 0.75f;

    [Header("Player Reference")]
    [SerializeField] private Transform playerRef;

    [Header("Gun Systems")]
    [SerializeField] private List<MCarRoofTurretGun_LevelSystem> guns = new List<MCarRoofTurretGun_LevelSystem>();

    [Header("PowerUp System")]
    [SerializeField] private PowerUpManager powerUpManager;

    private float currentExp = 0f;
    private float requiredExp;

    private void Start()
    {
        currentLevel = Mathf.Max(1, startingLevel);
        requiredExp = baseExpRequired;
        currentExp = 0f;

        SyncAllGuns();
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddExp(50);
        }
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        CheckLevelUp();
        UpdateUI();
    }

    private void CheckLevelUp()
    {
        while (currentExp >= requiredExp)
        {
            currentExp -= requiredExp;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        requiredExp *= expMultiplier;

        Debug.Log("LEVEL UP → Level: " + currentLevel);

        SyncAllGuns();
        CheckBossSpawns();

        if (powerUpManager != null)
        {
            powerUpManager.ActivatePowerUpOnLevelUp(currentLevel);
        }

        SpawnLevelUpEffect();
        UpdateUI();
    }

    private void CheckBossSpawns()
    {
        foreach (var boss in bosses)
        {
            if (boss == null) continue;

            if (!boss.hasSpawned && currentLevel >= boss.spawnLevel)
            {
                SpawnBoss(boss);
                boss.hasSpawned = true;
            }
        }
    }

    private void SpawnBoss(BossSpawnData boss)
    {
        if (boss.bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab missing for: " + boss.bossName);
            return;
        }

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (boss.spawnPoint != null)
        {
            spawnPosition = boss.spawnPoint.position;
            spawnRotation = boss.spawnPoint.rotation;
        }
        else if (playerRef != null)
        {
            spawnPosition = playerRef.position + Vector3.forward * 5f;
            spawnRotation = Quaternion.identity;
        }
        else
        {
            spawnPosition = transform.position;
            spawnRotation = Quaternion.identity;
        }

        Instantiate(boss.bossPrefab, spawnPosition, spawnRotation);

        Debug.Log("🔥 BOSS SPAWNED → " + boss.bossName + " at level " + boss.spawnLevel);
    }

    private void SpawnLevelUpEffect()
    {
        if (levelUpEffectPrefab == null || playerRef == null) return;

        GameObject fx = Instantiate(levelUpEffectPrefab, playerRef);
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localRotation = Quaternion.identity;

        Destroy(fx, levelUpEffectDuration);
    }

    private void SyncAllGuns()
    {
        if (guns.Count == 0)
        {
            Debug.LogWarning("No guns assigned!");
            return;
        }

        foreach (var gun in guns)
        {
            if (gun != null)
            {
                gun.SyncLevel(currentLevel);
            }
        }
    }

    private void UpdateUI()
    {
        if (expBar != null)
        {
            expBar.fillAmount = Mathf.Clamp01(currentExp / requiredExp);
        }

        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
    }
}