using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

public class MCarRoofTurretGun_LevelSystem : MonoBehaviour
{
    #region References

    [Header("Gun Reference")]
    [SerializeField] private MCarRoofTurretGun gun;

    #endregion

    #region Level Settings

    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int unlockLevel = 1;

    #endregion

    #region Level Data

    [System.Serializable]
    public class LevelStats
    {
        public MCarRoofTurretGun.GunMode gunType;

        [Header("Stats")]
        public float fireRate;
        public float spread;
        public float damage; // 🔥 NEW
    }

    [Header("Levels Data")]
    [SerializeField] private List<LevelStats> levels = new List<LevelStats>();

    #endregion

    #region Live Debug Values

    [Header("Live Runtime Values")]
    [SerializeField] private bool isUnlocked;
    [SerializeField] private int resolvedLevelIndex;

    [SerializeField] private MCarRoofTurretGun.GunMode currentGunType;
    [SerializeField] private float currentFireRate;
    [SerializeField] private float currentSpread;
    [SerializeField] private float currentDamage;

    #endregion

    #region Reflection Fields

    private FieldInfo modeField;
    private FieldInfo rifleRateField;
    private FieldInfo machineRateField;
    private FieldInfo machineSpreadField;

    #endregion

    private void Awake()
    {
        #region Setup

        if (gun == null)
            gun = GetComponent<MCarRoofTurretGun>();

        CacheFields();

        #endregion
    }

    private void Start()
    {
        #region Init Apply

        ApplyLevelStats();

        #endregion
    }

    #region Reflection Setup

    private void CacheFields()
    {
        #region Cache Fields

        Type type = typeof(MCarRoofTurretGun);

        modeField = type.GetField("currentMode", BindingFlags.NonPublic | BindingFlags.Instance);
        rifleRateField = type.GetField("rifleFireRate", BindingFlags.NonPublic | BindingFlags.Instance);
        machineRateField = type.GetField("machineFireRate", BindingFlags.NonPublic | BindingFlags.Instance);
        machineSpreadField = type.GetField("machineSpread", BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion
    }

    #endregion

    #region Public API

    public void SyncLevel(int playerLevel)
    {
        #region Sync Level

        currentLevel = playerLevel;
        ApplyLevelStats();

        #endregion
    }

    public float GetCurrentDamage()
    {
        #region Getter

        if (!isUnlocked || levels.Count == 0)
            return 0f;

        int index = Mathf.Clamp(currentLevel - unlockLevel, 0, levels.Count - 1);
        return levels[index].damage;

        #endregion
    }

    #endregion

    #region Core Logic

    private void ApplyLevelStats()
    {
        #region Apply Stats

        isUnlocked = currentLevel >= unlockLevel;

        if (!isUnlocked)
        {
            SetGunActive(false);
            ResetDebugValues();
            return;
        }

        SetGunActive(true);

        if (levels.Count == 0) return;

        int index = currentLevel - unlockLevel;
        index = Mathf.Clamp(index, 0, levels.Count - 1);

        resolvedLevelIndex = index;

        LevelStats stats = levels[index];

        modeField.SetValue(gun, stats.gunType);

        currentGunType = stats.gunType;
        currentFireRate = stats.fireRate;
        currentSpread = stats.spread;
        currentDamage = stats.damage;

        if (stats.gunType == MCarRoofTurretGun.GunMode.Rifle)
        {
            rifleRateField.SetValue(gun, stats.fireRate);
        }
        else if (stats.gunType == MCarRoofTurretGun.GunMode.MachineGun)
        {
            machineRateField.SetValue(gun, stats.fireRate);
            machineSpreadField.SetValue(gun, stats.spread);
        }

        #endregion
    }

    private void SetGunActive(bool state)
    {
        #region Enable / Disable

        gun.enabled = state;

        #endregion
    }

    private void ResetDebugValues()
    {
        #region Reset

        resolvedLevelIndex = -1;

        currentGunType = default;
        currentFireRate = 0;
        currentSpread = 0;
        currentDamage = 0;

        #endregion
    }

    #endregion
}