using UnityEngine;

public class EnemyDeathReporter : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private EnemiesList enemiesList;

    void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }   
        
        enemiesList = EnemiesList.instance;
        health.OnDeath += ReportDeath;
        enemiesList.AddEnemy(gameObject);
    }

    void ReportDeath()
    {
        enemiesList.RemoveEnemy(gameObject);
    }

    
}