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

    #region Level Settings

    [Header("Level System")]

    // 🧠 NEW: starting level control
    [SerializeField] private int startingLevel = 1;

    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float baseExpRequired = 100f;
    [SerializeField] private float expMultiplier = 1.25f;

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

    private void Start()
    {
        #region Initialize

        // 🧠 APPLY STARTING LEVEL
        currentLevel = Mathf.Max(1, startingLevel);

        requiredExp = baseExpRequired;
        currentExp = 0f;

        SyncAllGuns();

        UpdateUI();

        #endregion
    }

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

        #region PowerUps

        if (powerUpManager != null)
        {
            powerUpManager.ActivatePowerUpOnLevelUp();
        }

        #endregion

        UpdateUI();

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

    #region Debug

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
}