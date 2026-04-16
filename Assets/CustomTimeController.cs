using UnityEngine;

public class CustomTimeController : MonoBehaviour
{
    #region Settings

    [SerializeField] private float slowTimeScale = 0.3f;
    [SerializeField] private float transitionSpeed = 5f;

    #endregion

    #region Runtime

    private float currentScale = 1f;
    private float targetScale = 1f;
    private bool isSlowMotionActive = false;

    private VehicleTimeScaler[] vehicles;

    #endregion

    private void Start()
    {
        #region Cache Vehicles

        vehicles = FindObjectsOfType<VehicleTimeScaler>();

        #endregion
    }

    private void Update()
    {
        #region Input

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q PRESSED");
            ToggleSlowMotion();
        }

        #endregion

        #region Smooth Transition

        currentScale = Mathf.Lerp(currentScale, targetScale, transitionSpeed * Time.deltaTime);

        ApplyTimeScale(currentScale);

        #endregion
    }

    private void ToggleSlowMotion()
    {
        #region Toggle Logic

        isSlowMotionActive = !isSlowMotionActive;

        targetScale = isSlowMotionActive ? slowTimeScale : 1f;

        #endregion
    }

    private void ApplyTimeScale(float scale)
    {
        #region Apply To Vehicles

        foreach (VehicleTimeScaler v in vehicles)
        {
            v.SetTimeScale(scale);
        }

        #endregion
    }
}