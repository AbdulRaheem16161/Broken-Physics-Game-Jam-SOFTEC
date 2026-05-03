using UnityEngine;
using System;

public class KillsCounter : MonoBehaviour
{
   [SerializeField] private int numOfKills;
    public Action OnIncreaseKillCount;

   private void Start()
   {
       EnemiesList.instance.OnEnemyRemoval += IncreaseKillsCount;
   }

    private void IncreaseKillsCount()
    {
        numOfKills++;
        OnIncreaseKillCount?.Invoke(); 
    }

    public int GetNumOfKills()
    {
        return numOfKills;
    }
}
