using UnityEngine;

public class GlobalSlowMotion : MonoBehaviour
{
    #region Settings

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
        #region Input

        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleSlowMotion();
        }

        #endregion

        #region Smooth Transition

        currentScale = Mathf.Lerp(currentScale, targetScale, transitionSpeed * Time.unscaledDeltaTime);

        Time.timeScale = currentScale;

        // IMPORTANT: Keep physics stable
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        #endregion
    }

    private void ToggleSlowMotion()
    {
        #region Toggle Logic

        isSlowMotionActive = !isSlowMotionActive;

        targetScale = isSlowMotionActive ? slowTimeScale : 1f;

        #endregion
    }
}