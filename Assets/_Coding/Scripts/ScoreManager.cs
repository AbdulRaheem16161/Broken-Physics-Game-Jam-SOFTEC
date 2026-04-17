// ScoreManager.cs
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

    [Header("KPM Tracking")]
    [SerializeField] private float kpmWindowSeconds = 60f;

    #endregion

    #region Runtime Values

    public List<GameObject> enemies = new List<GameObject>();
    private int score = 0;
    private List<float> killTimestamps = new List<float>();

    #endregion

    #region Unity Methods

    private void Start()
    {
        #region Init
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

        #region Prune Old Timestamps

        float cutoff = Time.time - kpmWindowSeconds;
        killTimestamps.RemoveAll(t => t < cutoff);

        #endregion
    }

    #endregion

    #region Public Methods

    public void AddScore(int amount)
    {
        #region Add Score

        score += amount;
        killTimestamps.Add(Time.time);
        UpdateUI();

        if (expManager != null)
        {
            expManager.AddExp(expPerKill);
        }

        #endregion
    }

    #endregion

    #region Public Accessors

    public int KillCount => score;

    public float GetKillsPerMinute()
    {
        #region Calculate KPM

        float cutoff = Time.time - kpmWindowSeconds;
        int recentKills = killTimestamps.FindAll(t => t >= cutoff).Count;
        return recentKills * (60f / kpmWindowSeconds);

        #endregion
    }

    #endregion

    #region Private Methods

    private void UpdateUI()
    {
        #region UI Update

        if (scoreText != null)
        {
            scoreText.text = "Kills: " + score;
        }

        #endregion
    }

    #endregion
}