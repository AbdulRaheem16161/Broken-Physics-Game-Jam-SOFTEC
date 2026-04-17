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

    #region Level System

    [Header("Level System")]
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
    }

    [Header("Levels Data")]
    [SerializeField] private List<LevelStats> levels = new List<LevelStats>();

    #endregion

    #region Reflection Cache

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
        ApplyLevelStats();

        #endregion
    }

    #region Reflection Setup

    private void CacheFields()
    {
        #region Cache Private Fields

        Type type = typeof(MCarRoofTurretGun);

        modeField = type.GetField("currentMode", BindingFlags.NonPublic | BindingFlags.Instance);
        rifleRateField = type.GetField("rifleFireRate", BindingFlags.NonPublic | BindingFlags.Instance);
        machineRateField = type.GetField("machineFireRate", BindingFlags.NonPublic | BindingFlags.Instance);
        machineSpreadField = type.GetField("machineSpread", BindingFlags.NonPublic | BindingFlags.Instance);

        #endregion
    }

    #endregion

    #region Level Logic

    public void LevelUp()
    {
        #region Increase Level

        currentLevel++;

        Debug.Log("Gun Level Up → " + currentLevel);

        ApplyLevelStats();

        #endregion
    }

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

        int index = currentLevel - 2; // because level 2 = index 0

        if (index < 0 || index >= levels.Count)
            return;

        LevelStats stats = levels[index];

        // Set gun mode
        modeField.SetValue(gun, stats.gunType);

        // Apply based on mode
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
        #region Enable/Disable Gun

        gun.enabled = state;

        #endregion
    }

    #endregion
}