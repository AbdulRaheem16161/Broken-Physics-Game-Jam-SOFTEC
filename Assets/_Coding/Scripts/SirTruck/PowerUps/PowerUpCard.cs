using UnityEngine;
using UnityEngine.UI;
using System;

public class PowerUpCard : MonoBehaviour
{
    [SerializeField] private Image powerUpImage;
    [SerializeField] private BasePowerUp powerUp;
    public Action onCardSelected;

    private void Start()
    {
        Debug.Log("PowerUpCard script is running");
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space)) ///////////////////////
        //{
        //    Debug.Log("Space key pressed");
        //    SelectCard();
        //}
    }
    public void SelectCard()
    {
        Debug.Log("Selected Power-Up: " + powerUp.name + " now invoking OnCardSelected");

        if (powerUp == null)
        {
            Debug.LogWarning("PowerUp is null, cannot activate.");
            return;
        }
        powerUp.ActivatePowerUp();
        onCardSelected?.Invoke();
        Time.timeScale = 1f;
    }

    public void SetPowerUp(BasePowerUp newPowerUp)
    {
        powerUp = newPowerUp;

        if (powerUpImage == null)
        {
            Debug.LogError("PowerUpCard: PowerUpImage reference is missing!");
        }

        if (powerUp != null)
        {
            Debug.Log("Setting Power-Up: " + powerUp.name);
        }
        
        if (newPowerUp == null)
        {
            Debug.LogWarning("newPowerUp is null, cannot set image.");
        }
        powerUpImage.sprite = newPowerUp.powerUpImage;
    }
}
