using UnityEngine;

public class ShotGun : GunBase
{
    private void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        base.Update();
    }

    protected override void Shoot()
    {
        Vector3 direction = (closestEnemy.transform.position - firePoint.transform.position).normalized;

        for (int i = 0; i < numOfBullets; i++)
        {
            float angle = (i - (numOfBullets - 1) / 2f) * spreadMultiplier;

            Vector3 spreadDirection =
                Quaternion.Euler(0, angle, 0) * direction;

            GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.transform.position,
                Quaternion.identity
            );

            Projectile projectile = bullet.GetComponent<Projectile>();
            if (projectile != null)
            {
                Debug.Log("Setting projectile damage to: " + damage);
                projectile.damage = damage;
            }
            else
            {
                Debug.LogWarning("Projectile component not found on bullet prefab.");
            }

            bullet.GetComponent<Rigidbody>().linearVelocity =
                spreadDirection * projectileSpeed;
        }
    }
}
