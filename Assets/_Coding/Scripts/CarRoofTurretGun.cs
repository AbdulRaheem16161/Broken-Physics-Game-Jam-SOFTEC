using System.Collections;
using UnityEngine;

public class CarRoofTurretGun : MonoBehaviour
{
    public enum GunMode
    {
        Rifle,
        MachineGun,
        Shotgun,
        Cannon
    }

    [Header("Gun Mode")]
    [SerializeField] private GunMode currentMode;

    [Header("Targeting")]
    [SerializeField] private float detectionRadius = 25f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private string aimChildTag = "AimPoint";
    [SerializeField] private Transform turretHead;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float returnSpeed = 4f;
    [SerializeField] private Vector3 defaultLocalEuler;

    [Header("Fire Point")]
    [SerializeField] private Transform muzzlePoint;

    [Header("Projectile Speed")]
    [SerializeField] private float projectileSpeed = 40f;

    [Header("Projectiles")]
    [SerializeField] private GameObject rifleProjectile;
    [SerializeField] private GameObject machineProjectile;
    [SerializeField] private GameObject shotgunProjectile;
    [SerializeField] private GameObject cannonProjectile;

    [Header("Rifle")]
    [SerializeField] private float rifleFireRate = 0.25f;
    private float rifleTimer;

    [Header("Machine Gun")]
    [SerializeField] private float machineFireRate = 0.08f;
    [SerializeField] private float machineSpread = 3f;
    private float machineTimer;

    [Header("Shotgun")]
    [SerializeField] private float shotgunCooldown = 1.2f;
    [SerializeField] private int shotgunPellets = 6;
    [SerializeField] private float shotgunSpread = 8f;
    private float shotgunTimer;

    [Header("Cannon")]
    [SerializeField] private float cannonCooldown = 2.5f;
    private float cannonTimer;

    private Transform currentTarget;
    private Transform currentAimPoint;

    private void Update()
    {
        FindClosestTarget();
        UpdateAimPoint();
        RotateTurret();
        HandleShooting();
    }

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

    private void UpdateAimPoint()
    {
        currentAimPoint = null;

        if (currentTarget == null) return;

        Transform[] children = currentTarget.GetComponentsInChildren<Transform>();

        foreach (Transform t in children)
        {
            if (t.CompareTag(aimChildTag))
            {
                currentAimPoint = t;
                return;
            }
        }

        currentAimPoint = currentTarget;
    }

    private void RotateTurret()
    {
        if (currentAimPoint != null)
        {
            Vector3 dir = (currentAimPoint.position - turretHead.position).normalized;

            Quaternion lookRot = Quaternion.LookRotation(dir);

            turretHead.rotation = Quaternion.Slerp(
                turretHead.rotation,
                lookRot,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            Quaternion defaultRot = Quaternion.Euler(defaultLocalEuler);

            turretHead.localRotation = Quaternion.Slerp(
                turretHead.localRotation,
                defaultRot,
                returnSpeed * Time.deltaTime
            );
        }
    }

    private void HandleShooting()
    {
        if (currentAimPoint == null) return;

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

    private void Rifle()
    {
        rifleTimer += Time.deltaTime;

        if (rifleTimer >= rifleFireRate)
        {
            rifleTimer = 0f;
            Fire(rifleProjectile, 0f);
        }
    }

    private void MachineGun()
    {
        machineTimer += Time.deltaTime;

        if (machineTimer >= machineFireRate)
        {
            machineTimer = 0f;
            Fire(machineProjectile, machineSpread);
        }
    }

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

    private void Cannon()
    {
        cannonTimer += Time.deltaTime;

        if (cannonTimer >= cannonCooldown)
        {
            cannonTimer = 0f;
            Fire(cannonProjectile, 0f);
        }
    }

    private void Fire(GameObject prefab, float spread)
    {
        if (prefab == null || muzzlePoint == null || currentAimPoint == null) return;

        Vector3 direction = (currentAimPoint.position - muzzlePoint.position).normalized;

        direction += new Vector3(
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f
        );

        GameObject projectile = Instantiate(
            prefab,
            muzzlePoint.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        Projectile proj = projectile.GetComponent<Projectile>();

        if (proj != null)
        {
            proj.attacker = transform.root.gameObject;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}