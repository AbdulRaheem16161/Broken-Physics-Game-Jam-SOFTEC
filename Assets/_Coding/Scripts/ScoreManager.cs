using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    #region Settings

    [Header("Enemy Settings")]
    [SerializeField] private string enemyTag = "Enemy";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("XP System")]
    [SerializeField] private ExpManager expManager;
    [SerializeField] private int expPerKill = 10;

    #endregion

    #region Runtime Values

    private List<GameObject> enemies = new List<GameObject>();
    private int score = 0;

    #endregion

    #region Unity Methods

    private void Start()
    {
        #region Initialize Score

        score = 0;
        UpdateUI();

        #endregion
    }

    private void Update()
    {
        #region Track Enemies

        // NOTE: This is still not ideal performance-wise,
        // but keeping your original logic intact.

        GameObject[] foundEnemies = GameObject.FindGameObjectsWithTag(enemyTag);

        enemies.Clear();
        enemies.AddRange(foundEnemies);

        #endregion
    }

    #endregion

    #region Public Methods

    public void AddScore(int amount)
    {
        #region Add Score

        score += amount;
        UpdateUI();

        // Send XP to ExpManager
        if (expManager != null)
        {
            expManager.AddExp(expPerKill);
        }

        #endregion
    }

    #endregion

    #region Private Methods

    private void UpdateUI()
    {
        #region Update UI Text

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        #endregion
    }

    #endregion
}