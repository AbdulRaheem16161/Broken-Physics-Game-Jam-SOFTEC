using UnityEngine;
using UnityEngine.UI;
using System;

public class ExpUI : MonoBehaviour
{
   [SerializeField] private LevelUpManager LevelUpManager;
   [SerializeField] private Image expBarFill;
    [SerializeField] private TMPro.TextMeshProUGUI currentLevelText;
   
   private void Awake()
    {
        LevelUpManager.OnExpChange += UpdateUI; 
        UpdateUI(); 
    }
    private void UpdateUI()
    {
        expBarFill.fillAmount = LevelUpManager.GetCurrentExp() / LevelUpManager.GetExpToLevelUp();
        currentLevelText.text = "Level: " + LevelUpManager.GetPlayerLevel().ToString();
    }
}
