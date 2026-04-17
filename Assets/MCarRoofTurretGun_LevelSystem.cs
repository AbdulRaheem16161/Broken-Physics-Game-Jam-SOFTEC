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

    // Current level of THIS gun (syncs with player level)
    [SerializeField] private int currentLevel = 1;

    // Level at which gun unlocks
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
    }

    [Header("Levels Data")]
    [SerializeField] private List<LevelStats> levels = new List<LevelStats>();

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

    #endregion

    #region Core Logic

    private void ApplyLevelStats()
    {
        #region Apply Stats

        // LOCK SYSTEM
        if (currentLevel < unlockLevel)
        {
            SetGunActive(false);
            return;
        }

        SetGunActive(true);

        // 🔥 RELATIVE INDEX (FIXED SYSTEM)
        int index = currentLevel - unlockLevel;

        // Clamp so it reaches max properly
        index = Mathf.Clamp(index, 0, levels.Count - 1);

        if (levels.Count == 0) return;

        LevelStats stats = levels[index];

        // Apply gun mode
        modeField.SetValue(gun, stats.gunType);

        // Apply stats
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

    #endregion
}