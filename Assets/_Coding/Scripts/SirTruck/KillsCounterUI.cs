using UnityEngine;

public class KillsCounterUI : MonoBehaviour
{
   [SerializeField] private KillsCounter killsCounter;
   [SerializeField] private TMPro.TextMeshProUGUI killsCounterText;
   private void Awake()
    {
        killsCounter.OnIncreaseKillCount += UpdateUI;

    }
    private void UpdateUI()
    {
        killsCounterText.text = "Kills: " + killsCounter.GetNumOfKills().ToString();
    }
}
