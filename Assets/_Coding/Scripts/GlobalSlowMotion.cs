using UnityEngine;

public class GlobalSlowMotion : MonoBehaviour
{
    #region Settings

    [Header("Debug Input")]
    public bool WillFreezeOnPressingL = false;

    [Header("Slow Motion Settings")]
    [SerializeField] private float slowTimeScale = 0.2f;
    [SerializeField] private float transitionSpeed = 5f;

    #endregion

    #region Runtime

    private float currentScale = 1f;
    private float targetScale = 1f;
    private bool isSlowMotionActive = false;

    #endregion

    private void Update()
    {
        #region Debug Input Toggle

        if (WillFreezeOnPressingL && Input.GetKeyDown(KeyCode.L))
        {
            ToggleSlowMotion();
        }

        #endregion

        #region Smooth Time Transition

        currentScale = Mathf.Lerp(currentScale, targetScale, transitionSpeed * Time.unscaledDeltaTime);

        Time.timeScale = currentScale;

        // Keep physics stable
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        #endregion
    }

    #region Toggle (Debug Only)

    private void ToggleSlowMotion()
    {
        isSlowMotionActive = !isSlowMotionActive;

        targetScale = isSlowMotionActive ? slowTimeScale : 1f;
    }

    #endregion

    #region PUBLIC POWERUP CONTROLS

    public void EnableSlowMotion()
    {
        #region Enable

        isSlowMotionActive = true;
        targetScale = slowTimeScale;

        #endregion
    }

    public void DisableSlowMotion()
    {
        #region Disable

        isSlowMotionActive = false;
        targetScale = 1f;

        #endregion
    }

    #endregion
}