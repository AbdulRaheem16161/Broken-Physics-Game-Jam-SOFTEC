using UnityEngine;

public class EnemyDeathReporter : MonoBehaviour
{
    #region Runtime

    private ScoreManager scoreManager;

    #endregion

    #region Unity Methods

    private void Start()
    {
        #region Find Score Manager

        scoreManager = FindObjectOfType<ScoreManager>();

        #endregion
    }

    private void OnDestroy()
    {
        #region Notify Score Manager

        if (scoreManager != null)
        {
            scoreManager.AddScore(1);
        }

        #endregion
    }

    #endregion
}