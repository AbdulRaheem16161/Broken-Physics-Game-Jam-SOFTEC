using UnityEngine;
using UnityEngine.EventSystems;

public class SteeringTouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum SteeringDirection
    {
        Left,
        Right
    }

    [SerializeField] private SteeringDirection direction;
    [SerializeField] private PlayerTouchControls playerTouchControls;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (direction == SteeringDirection.Left)
        {
            playerTouchControls.PressLeft();
        }
        else
        {
            playerTouchControls.PressRight();
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        playerTouchControls.ReleaseSteering();
    }
}