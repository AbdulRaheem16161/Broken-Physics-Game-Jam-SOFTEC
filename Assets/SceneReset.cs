using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    #region Settings

    [Header("Reset Settings")]

    // Key used to reset the scene
    [SerializeField] private KeyCode resetKey = KeyCode.R;

    // Time (in seconds) the key must be held
    [SerializeField] private float holdDuration = 2f;

    // If true, resetting is disabled
    [SerializeField] private bool disableReset = false;

    #endregion


    #region Runtime Values

    // Timer to track how long the key is held
    private float holdTimer = 0f;

    #endregion


    private void Update()
    {
        #region Update Method

        HandleResetInput();

        #endregion
    }


    private void HandleResetInput()
    {
        #region Handle Reset Input

        // If reset is disabled, do nothing
        if (disableReset)
            return;

        // If key is being held
        if (Input.GetKey(resetKey))
        {
            // Increase timer
            holdTimer += Time.deltaTime;

            // If held long enough → reset scene
            if (holdTimer >= holdDuration)
            {
                ResetCurrentScene();
            }
        }
        else
        {
            // Reset timer if key is released early
            holdTimer = 0f;
        }

        #endregion
    }


    private void ResetCurrentScene()
    {
        #region Reset Current Scene

        // Reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        #endregion
    }


    #region Public Methods

    // Enable or disable reset from other scripts
    public void SetResetEnabled(bool isEnabled)
    {
        disableReset = !isEnabled;
    }

    #endregion
}