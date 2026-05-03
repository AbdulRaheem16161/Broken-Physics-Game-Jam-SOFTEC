using UnityEngine;
using System.Collections.Generic;   
using System;


public class EnemiesList : MonoBehaviour
{
    public static EnemiesList instance;
    public List<GameObject> enemyList = new List<GameObject>();

    public Action OnEnemyRemoval;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);

            return;
        }

        instance = this;
    }

    public void AddEnemy(GameObject enemy)
    {
       enemyList.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemyList.Remove(enemy);
        OnEnemyRemoval?.Invoke();
    }
}
