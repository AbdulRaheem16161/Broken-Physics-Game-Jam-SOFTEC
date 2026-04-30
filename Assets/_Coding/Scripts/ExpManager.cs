using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    [Header("Level 10 Spawn")]
    [SerializeField] private GameObject level10SpawnPrefab;
    [SerializeField] private Transform level10SpawnPoint;

    private bool level10Spawned = false;

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

        if (currentLevel >= 10 && !level10Spawned)
        {
            SpawnLevel10Object();
            level10Spawned = true;
        }

        if (powerUpManager != null)
        {
            powerUpManager.ActivatePowerUpOnLevelUp(currentLevel);
        }

        SpawnLevelUpEffect();
        UpdateUI();
    }

    private void SpawnLevel10Object()
    {
        if (level10SpawnPrefab == null)
        {
            Debug.LogWarning("Level 10 prefab not assigned!");
            return;
        }

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (level10SpawnPoint != null)
        {
            spawnPosition = level10SpawnPoint.position;
            spawnRotation = level10SpawnPoint.rotation;
        }
        else
        {
            if (playerRef != null)
            {
                spawnPosition = playerRef.position;
                spawnRotation = Quaternion.identity;
            }
            else
            {
                spawnPosition = transform.position;
                spawnRotation = Quaternion.identity;
            }
        }

        Instantiate(level10SpawnPrefab, spawnPosition, spawnRotation);

        Debug.Log("🔥 LEVEL 10 → Special object spawned!");
    }

    private void SpawnLevelUpEffect()
    {
        if (levelUpEffectPrefab == null) return;
        if (playerRef == null) return;

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