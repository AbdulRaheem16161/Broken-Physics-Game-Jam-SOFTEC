using UnityEngine;

public class GunBase : MonoBehaviour
{
    [SerializeField] protected GameObject firePoint;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected EnemiesList enemiesList; 
    [SerializeField] protected GameObject closestEnemy;
    [SerializeField] protected bool enemyIsWithinAngle;
    [SerializeField] protected float closestDistance;
    [SerializeField] protected float timeLeftForNextFire;     
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float shootRange = 20f;
    [SerializeField] protected float maxGunAngle = 45f;
    [SerializeField] protected float minGunAngle = 45f;
    [SerializeField] protected float projectileSpeed = 20f;
    [SerializeField] protected int numOfBullets = 1;
    [SerializeField] protected float spreadMultiplier = 0.1f;
    [SerializeField] protected float damage = 10f;

    [SerializeField] protected bool ShowGizmos = true;

    protected void Awake()
    {
        if (enemiesList == null)    
        {
            enemiesList = EnemiesList.instance;
        }
    }

    protected void Update()
    {
        FindClossestEnemy();
        CheckAngleToEnemy();

        if (closestEnemy != null && closestDistance <= shootRange && enemyIsWithinAngle)
        {
            timeLeftForNextFire -= Time.deltaTime;
            if (timeLeftForNextFire <= 0f)
            {
                Shoot();
                timeLeftForNextFire = fireRate; 
            }
        }
    }

    private void FindClossestEnemy()
    {
        if (enemiesList == null || enemiesList.enemyList == null || enemiesList.enemyList.Count == 0) return;

        closestEnemy = null;
        closestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemiesList.enemyList)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
    }

    private void CheckAngleToEnemy()
    {
        if (closestEnemy == null) return;

        Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
        float angleToEnemy = Vector3.Angle(transform.forward, directionToEnemy);

        if (angleToEnemy <= maxGunAngle / 2f && angleToEnemy >= minGunAngle / 2f)
        {
            enemyIsWithinAngle = true;
        }
        else
        {
            enemyIsWithinAngle = false;
        }
    }

    protected void OnDrawGizmos()
    {
        if (!ShowGizmos) return;
        
        Gizmos.color = Color.red;

        Vector3 leftDirection =
            Quaternion.Euler(0, maxGunAngle / 2f, 0) * transform.forward;

        Vector3 rightDirection =
            Quaternion.Euler(0, minGunAngle / 2f, 0) * transform.forward;

        Gizmos.DrawLine(
            transform.position,
            transform.position + leftDirection * shootRange
        );

        Gizmos.DrawLine(
            transform.position,
            transform.position + rightDirection * shootRange
        );
    }

    protected virtual void Shoot()
    {
    }
}
