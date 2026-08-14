using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpsManager : MonoBehaviour
{
    [SerializeField] private List<BasePowerUp> powerUps = new List<BasePowerUp>();
    [SerializeField] private List<PowerUpCard> powerUpCards = new List<PowerUpCard>();
    [SerializeField] private LevelUpManager LevelUpManager;
    [SerializeField] private int firstCardPowerUpIndex;
    [SerializeField] private int secondCardPowerUpIndex;
    [SerializeField] private int thirdCardPowerUpIndex;

    private void Start()
    {
        LevelUpManager.OnLevelUp += ActivatePowerUp;

        foreach (var card in powerUpCards)
        {
            card.onCardSelected += DisablePowerUps;
            card.gameObject.SetActive(false);
        }
    }

    private void ActivatePowerUp()
    {
        SelectRandomPowerUps();
        EnablePowerUps();
    }

    private void SelectRandomPowerUps()
    {
        firstCardPowerUpIndex = Random.Range(0, powerUps.Count);

        secondCardPowerUpIndex = Random.Range(0, powerUps.Count);
        while (firstCardPowerUpIndex == secondCardPowerUpIndex)
        {
            secondCardPowerUpIndex = Random.Range(0, powerUps.Count);
        }

        thirdCardPowerUpIndex = Random.Range(0, powerUps.Count);
        while (thirdCardPowerUpIndex == firstCardPowerUpIndex || thirdCardPowerUpIndex == secondCardPowerUpIndex)
        {
            thirdCardPowerUpIndex = Random.Range(0, powerUps.Count);
        }
    }

    private void EnablePowerUps()
    {
        powerUpCards[0].SetPowerUp(powerUps[firstCardPowerUpIndex]);
        powerUpCards[1].SetPowerUp(powerUps[secondCardPowerUpIndex]);
        powerUpCards[2].SetPowerUp(powerUps[thirdCardPowerUpIndex]);

        foreach (var card in powerUpCards)
        {
            card.gameObject.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void DisablePowerUps()
    {
        Debug.Log("PowerUp Selected, disabling cards...");

        // powerUpCards[0].SetPowerUp(null);
        // powerUpCards[1].SetPowerUp(null);
        // powerUpCards[2].SetPowerUp(null);

        
        foreach (var card in powerUpCards)
        {
            card.gameObject.SetActive(false);
        }
    }



}