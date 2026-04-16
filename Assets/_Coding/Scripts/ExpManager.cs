using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpManager : MonoBehaviour
{
    #region UI References

    [Header("UI")]
    [SerializeField] private Image expBar;
    [SerializeField] private TextMeshProUGUI levelText;

    #endregion

    #region Level Settings

    [Header("Level System")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private float baseExpRequired = 100f;
    [SerializeField] private float expMultiplier = 1.25f;

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

        requiredExp = baseExpRequired;
        currentExp = 0f;

        UpdateUI();

        #endregion
    }

    #region Public Methods

    public void AddExp(int amount)
    {
        #region Add XP

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

        Debug.Log("Level Up! New Level: " + currentLevel);

        // 🔥 TRIGGER POWERUPS HERE
        if (powerUpManager != null)
        {
            Debug.Log("Calling PowerUp Manager...");
            powerUpManager.ActivatePowerUpOnLevelUp();
        }
        else
        {
            Debug.LogWarning("PowerUpManager not assigned!");
        }

        UpdateUI();

        #endregion
    }

    #endregion

    #region UI Update

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