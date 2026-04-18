using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ExpManager : MonoBehaviour
{
    #region UI References

    [Header("UI")]
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI levelText;

    #endregion

    #region Level System

    [Header("Level System")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float baseExpRequired = 100f;
    [SerializeField] private float expMultiplier = 1.25f;

    #endregion

    #region Level 10 Spawn (NEW 🔥)

    [Header("Level 10 Spawn")]
    [SerializeField] private GameObject level10SpawnPrefab;
    [SerializeField] private Transform level10SpawnPoint;

    private bool level10Spawned = false;

    #endregion

    #region Level Up FX

    [Header("Level Up Effect")]
    [SerializeField] private GameObject levelUpEffectPrefab;
    [SerializeField] private float levelUpEffectDuration = 0.75f;

    [Header("Player Reference")]
    [SerializeField] private Transform playerRef;

    #endregion

    #region Gun Systems

    [Header("Gun Systems")]
    [SerializeField] private List<MCarRoofTurretGun_LevelSystem> guns = new List<MCarRoofTurretGun_LevelSystem>();

    #endregion

    #region PowerUp System

    [Header("PowerUp System")]
    [SerializeField] private PowerUpManager powerUpManager;

    #endregion

    #region Runtime Values

    private float currentExp = 0f;
    private float requiredExp;

    #endregion

    #region Unity Methods

    private void Start()
    {
        #region Initialize

        currentLevel = Mathf.Max(1, startingLevel);

        requiredExp = baseExpRequired;
        currentExp = 0f;

        SyncAllGuns();
        UpdateUI();

        #endregion
    }

    private void Update()
    {
        #region Debug Key

        if (Input.GetKeyDown(KeyCode.L))
        {
            AddExp(50);
        }

        #endregion
    }

    #endregion

    #region Public Methods

    public void AddExp(int amount)
    {
        #region Add EXP

        currentExp += amount;

        CheckLevelUp();
        UpdateUI();

        #endregion
    }

    #endregion

    #region Level System

    private void CheckLevelUp()
    {
        #region Check Level Up

        while (currentExp >= requiredExp)
        {
            currentExp -= requiredExp;
            LevelUp();
        }

        #endregion
    }

    private void LevelUp()
    {
        #region Level Up

        currentLevel++;
        requiredExp *= expMultiplier;

        Debug.Log("LEVEL UP → Level: " + currentLevel);

        SyncAllGuns();

        #region Level 10 Spawn Check

        if (currentLevel >= 10 && !level10Spawned)
        {
            SpawnLevel10Object();
            level10Spawned = true;
        }

        #endregion

        #region PowerUps

        if (powerUpManager != null)
        {
            powerUpManager.ActivatePowerUpOnLevelUp();
        }

        #endregion

        #region Level Up FX

        SpawnLevelUpEffect();

        #endregion

        UpdateUI();

        #endregion
    }

    #endregion

    #region Level 10 Spawn Logic

    private void SpawnLevel10Object()
    {
        #region Safety Check

        if (level10SpawnPrefab == null)
        {
            Debug.LogWarning("Level 10 prefab not assigned!");
            return;
        }

        #endregion

        #region Determine Spawn Position

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (level10SpawnPoint != null)
        {
            spawnPosition = level10SpawnPoint.position;
            spawnRotation = level10SpawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("No spawn point assigned. Spawning at player.");

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

        #endregion

        #region Spawn

        Instantiate(level10SpawnPrefab, spawnPosition, spawnRotation);

        Debug.Log("🔥 LEVEL 10 → Special object spawned!");

        #endregion
    }

    #endregion

    #region Level Up FX Logic

    private void SpawnLevelUpEffect()
    {
        #region Guard

        if (levelUpEffectPrefab == null) return;
        if (playerRef == null) return;

        #endregion

        #region Spawn + Attach

        GameObject fx = Instantiate(levelUpEffectPrefab, playerRef);
        fx.transform.localPosition = Vector3.zero;
        fx.transform.localRotation = Quaternion.identity;

        #endregion

        #region Destroy After Time

        Destroy(fx, levelUpEffectDuration);

        #endregion
    }

    #endregion

    #region Gun Sync

    private void SyncAllGuns()
    {
        #region Sync Guns

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

        #endregion
    }

    #endregion

    #region UI

    private void UpdateUI()
    {
        #region EXP Bar

        if (expBar != null)
        {
            expBar.fillAmount = Mathf.Clamp01(currentExp / requiredExp);
        }

        #endregion

        #region Level Text

        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }

        #endregion
    }

    #endregion
}