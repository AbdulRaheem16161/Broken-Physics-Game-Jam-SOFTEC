using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    public KillsCounter killsCounter;
 
    [SerializeField] private int playerLevel = 1;
    [SerializeField] private float expPerKill = 10;
    [SerializeField] private float expToLevelUp = 30;
    [SerializeField] private float currentExp;

    [SerializeField] private float expReqPerLevelMult = 1.25f;

    public System.Action OnExpChange;
    public System.Action OnLevelUp;

    private void Awake()
    {
        if (killsCounter == null)
        {
           killsCounter = GetComponent<KillsCounter>();
        }

        killsCounter.OnIncreaseKillCount += AddExp;
    }

    public void AddExp()
    {
        currentExp += expPerKill;
        if (currentExp >= expToLevelUp)
        {
            LevelUp();
        }
        OnExpChange?.Invoke();
    }
    

    private void LevelUp()
    {
        playerLevel++;
        currentExp = 0;
        expToLevelUp *= expReqPerLevelMult;  
        OnExpChange?.Invoke();
        OnLevelUp?.Invoke();
    }

    public float GetExpToLevelUp()
    {
        return expToLevelUp;
    }

    public float GetCurrentExp()
    {
        return currentExp;
    }

    public int GetPlayerLevel()
    {
        return playerLevel;
    }
}