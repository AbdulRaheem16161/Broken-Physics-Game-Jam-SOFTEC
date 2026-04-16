using UnityEngine;
using ArcadeVP;

public class PlayerTimeCompensator : MonoBehaviour
{
    #region References
    [SerializeField] private ArcadeVehicleController controller;
    #endregion

    #region Runtime
    private float baseSpeed;
    private float baseAcceleration;
    private float baseTurn;
    #endregion

    private void Awake()
    {
        if (controller == null)
        {
            controller = GetComponent<ArcadeVehicleController>();
        }

        baseSpeed = controller.maxSpeed;
        baseAcceleration = controller.acceleration;
        baseTurn = controller.turn;
    }

    private void Update()
    {
        #region Compensate Time

        float scale = 1f / Time.timeScale;

        controller.maxSpeed = baseSpeed * scale;
        controller.acceleration = baseAcceleration * scale;
        controller.turn = baseTurn * scale;

        #endregion
    }
}