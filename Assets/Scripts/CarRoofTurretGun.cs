using System.Collections;
using UnityEngine;

public class CarRoofTurretGun : MonoBehaviour
{
    #region Gun Mode

    public enum GunMode
    {
        Rifle,
        MachineGun,
        Shotgun,
        Cannon
    }

    [Header("Gun Mode")]
    [SerializeField] private GunMode currentMode;

    #endregion

    #region Targeting

    [Header("Targeting")]
    [SerializeField] private float detectionRadius = 25f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform turretHead;

    private Transform currentTarget;

    #endregion

    #region Rotation

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    #endregion

    #region Fire Points

    [Header("Fire Point")]
    [SerializeField] private Transform muzzlePoint;

    #endregion

    #region Projectiles

    [Header("Projectiles")]
    [SerializeField] private GameObject rifleProjectile;
    [SerializeField] private GameObject machineProjectile;
    [SerializeField] private GameObject shotgunProjectile;
    [SerializeField] private GameObject cannonProjectile;

    #endregion

    #region Rifle

    [Header("Rifle")]
    [SerializeField] private float rifleFireRate = 0.25f;
    private float rifleTimer;

    #endregion

    #region Machine Gun

    [Header("Machine Gun")]
    [SerializeField] private float machineFireRate = 0.08f;
    [SerializeField] private float machineSpread = 3f;
    private float machineTimer;

    #endregion

    #region Shotgun

    [Header("Shotgun")]
    [SerializeField] private float shotgunCooldown = 1.2f;
    [SerializeField] private int shotgunPellets = 6;
    [SerializeField] private float shotgunSpread = 8f;
    private float shotgunTimer;

    #endregion

    #region Cannon

    [Header("Cannon")]
    [SerializeField] private float cannonCooldown = 2.5f;
    private float cannonTimer;

    #endregion

    #region Unity

    private void Update()
    {
        FindClosestTarget();
        RotateTowardsTarget();
        HandleShooting();
    }

    #endregion

    #region Targeting

    private void FindClosestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
            }
        }

        currentTarget = closest;
    }

    #endregion

    #region Rotation

    private void RotateTowardsTarget()
    {
        if (currentTarget == null || turretHead == null) return;

        Vector3 dir = (currentTarget.position - turretHead.position).normalized;

        Quaternion lookRot = Quaternion.LookRotation(dir);

        turretHead.rotation = Quaternion.Slerp(
            turretHead.rotation,
            lookRot,
            rotationSpeed * Time.deltaTime
        );
    }

    #endregion

    #region Shooting Controller

    private void HandleShooting()
    {
        if (currentTarget == null) return;

        switch (currentMode)
        {
            case GunMode.Rifle:
                Rifle();
                break;

            case GunMode.MachineGun:
                MachineGun();
                break;

            case GunMode.Shotgun:
                Shotgun();
                break;

            case GunMode.Cannon:
                Cannon();
                break;
        }
    }

    #endregion

    #region Rifle

    private void Rifle()
    {
        rifleTimer += Time.deltaTime;

        if (rifleTimer >= rifleFireRate)
        {
            rifleTimer = 0f;
            Fire(rifleProjectile, 0f);
        }
    }

    #endregion

    #region Machine Gun

    private void MachineGun()
    {
        machineTimer += Time.deltaTime;

        if (machineTimer >= machineFireRate)
        {
            machineTimer = 0f;
            Fire(machineProjectile, machineSpread);
        }
    }

    #endregion

    #region Shotgun

    private void Shotgun()
    {
        shotgunTimer += Time.deltaTime;

        if (shotgunTimer >= shotgunCooldown)
        {
            shotgunTimer = 0f;
            StartCoroutine(ShotgunBurst());
        }
    }

    private IEnumerator ShotgunBurst()
    {
        for (int i = 0; i < shotgunPellets; i++)
        {
            Fire(shotgunProjectile, shotgunSpread);
            yield return null;
        }
    }

    #endregion

    #region Cannon

    private void Cannon()
    {
        cannonTimer += Time.deltaTime;

        if (cannonTimer >= cannonCooldown)
        {
            cannonTimer = 0f;
            Fire(cannonProjectile, 0f);
        }
    }

    #endregion

    #region Fire System

    private void Fire(GameObject prefab, float spread)
    {
        if (prefab == null || muzzlePoint == null || currentTarget == null) return;

        Vector3 direction = (currentTarget.position - muzzlePoint.position).normalized;

        direction += new Vector3(
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f
        );

        GameObject proj = Instantiate(prefab, muzzlePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direction * 40f;
        }

        // IMPORTANT: attacker assignment
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.attacker = transform.root.gameObject;
        }
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    #endregion
}