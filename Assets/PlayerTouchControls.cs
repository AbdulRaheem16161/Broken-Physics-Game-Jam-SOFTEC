using ArcadeVP;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTouchControls : MonoBehaviour
{
    [SerializeField] private ArcadeVehicleController arcadeVehicleController;

    [SerializeField] private float steeringValue;


    private void Update()
    {
        arcadeVehicleController.ProvideInputs(steeringValue, 1f, 0);
    }


    public void PressLeft()
    {
        steeringValue = -1f;
    }


    public void PressRight()
    {
        steeringValue = 1f;
    }


    public void ReleaseSteering()
    {
        steeringValue = 0f;
    }
}