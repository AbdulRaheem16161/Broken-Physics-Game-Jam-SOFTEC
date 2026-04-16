using UnityEngine;
using ArcadeVP;

public class VehicleTimeScaler : MonoBehaviour
{
    #region References
    [SerializeField] private ArcadeVehicleController vehicleController;
    #endregion

    #region Settings
    [SerializeField] private bool affectedByTimeSlow = true;
    #endregion

    #region Runtime
    private float normalMaxSpeed;
    private float currentTimeScale = 1f;
    #endregion

    private void Awake()
    {
        #region Cache Original Values

        if (vehicleController == null)
        {
            vehicleController = GetComponent<ArcadeVehicleController>();
        }

        normalMaxSpeed = vehicleController.maxSpeed;

        #endregion
    }

    private void Update()
    {
        #region Apply Time Scaling

        if (!affectedByTimeSlow)
        {
            // Always stay normal
            vehicleController.maxSpeed = normalMaxSpeed;
            return;
        }

        vehicleController.maxSpeed = normalMaxSpeed * currentTimeScale;

        #endregion
    }

    #region Public Methods

    public void SetTimeScale(float scale)
    {
        currentTimeScale = scale;
    }

    #endregion
}