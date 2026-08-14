using UnityEngine;

public class MachineGun : GunBase
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
        Vector3 direction =
            (closestEnemy.transform.position - firePoint.transform.position).normalized;

        for (int i = 0; i < numOfBullets; i++)
        {
            float randomYaw = Random.Range(-spreadMultiplier, spreadMultiplier);
            float randomPitch = Random.Range(-spreadMultiplier, spreadMultiplier);

            Vector3 spreadDirection =
                Quaternion.Euler(randomPitch, randomYaw, 0) * direction;

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
