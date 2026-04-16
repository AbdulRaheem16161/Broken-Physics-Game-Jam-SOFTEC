using UnityEngine;
using ArcadeVP;

public class PlayerCarSlowMotionStats : MonoBehaviour
{
    #region References
    [SerializeField] private ArcadeVehicleController controller;
    #endregion

    #region Settings - Normal Stats
    [Header("Normal Stats")]
    [SerializeField] private float normalMaxSpeed;
    [SerializeField] private float normalAcceleration;
    [SerializeField] private float normalTurn;
    #endregion

    #region Settings - Slow Motion Stats
    [Header("Slow Motion Stats")]
    [SerializeField] private float slowMaxSpeed = 120f;
    [SerializeField] private float slowAcceleration = 200f;
    [SerializeField] private float slowTurn = 150f;
    #endregion

    #region Runtime
    private bool isSlowMotion;
    #endregion

    private void Awake()
    {
        #region Auto Assign

        if (controller == null)
            controller = GetComponent<ArcadeVehicleController>();

        #endregion

        #region Cache Normal Values

        normalMaxSpeed = controller.maxSpeed;
        normalAcceleration = controller.acceleration;
        normalTurn = controller.turn;

        #endregion
    }

    private void Update()
    {
        #region Detect Time State

        isSlowMotion = Time.timeScale < 0.95f;

        ApplyStats();

        #endregion
    }

    private void ApplyStats()
    {
        #region Apply Correct Stats

        if (isSlowMotion)
        {
            controller.maxSpeed = slowMaxSpeed;
            controller.acceleration = slowAcceleration;
            controller.turn = slowTurn;
        }
        else
        {
            controller.maxSpeed = normalMaxSpeed;
            controller.acceleration = normalAcceleration;
            controller.turn = normalTurn;
        }

        #endregion
    }
}