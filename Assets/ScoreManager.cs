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

    #endregion

    #region Runtime Values

    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private int score = 0;

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

        #endregion
    }

    #endregion

    #region Private Methods

    private void UpdateUI()
    {
        #region Update UI Text

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }

        #endregion
    }

    #endregion
}