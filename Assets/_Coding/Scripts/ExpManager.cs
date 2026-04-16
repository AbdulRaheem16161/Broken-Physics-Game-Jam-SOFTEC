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

    #region Runtime Values

    private float currentExp = 0f;
    private float requiredExp;

    #endregion

    private void Start()
    {
        #region Initialize XP System

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
        #region Level Up Logic

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

        UpdateUI();

        Debug.Log("Level Up! New Level: " + currentLevel);

        #endregion
    }

    #endregion

    #region UI Update

    private void UpdateUI()
    {
        #region Update EXP Bar

        if (expBar != null)
        {
            float fillAmount = currentExp / requiredExp;
            expBar.fillAmount = Mathf.Clamp01(fillAmount);
        }

        #endregion

        #region Update Level Text

        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }

        #endregion
    }

    #endregion
}