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

    #region Gun Mode
    [Header("Gun Mode")]
    [SerializeField] private GunMode currentMode;
    #endregion

    #region Targeting
    [Header("Targeting")]
    [SerializeField] private float detectionRange = 25f;
    [SerializeField] [Range(1f, 360f)] private float detectionAngle = 90f;
    [SerializeField] private float pointBlankRange = 6f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private string aimChildTag = "AimPoint";
    [SerializeField] private Transform turretHead;
    #endregion

    #region Rotation
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float returnSpeed = 4f;
    [SerializeField] private Vector3 defaultLocalEuler;
    #endregion

    #region Fire Point
    [Header("Fire Point")]
    [SerializeField] private Transform muzzlePoint;
    #endregion

    #region Projectile Speed
    [Header("Projectile Speed")]
    [SerializeField] private float projectileSpeed = 40f;
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
    private float shotgunTimer;
    #endregion

    #region Cannon
    [Header("Cannon")]
    [SerializeField] private float cannonCooldown = 2.5f;
    private float cannonTimer;
    #endregion

    #region Runtime
    private Transform currentTarget;
    private Transform currentAimPoint;
    #endregion

    private void Update()
    {
        FindClosestTargetInCone();
        UpdateAimPoint();
        RotateTurret();
        HandleShooting();
    }

    private void FindClosestTargetInCone()
    {
        #region Find Target

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, targetLayer);

        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Collider hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);

            if (dist > pointBlankRange)
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);
                if (angle > detectionAngle * 0.5f) continue;
            }

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.transform;
            }
        }

        currentTarget = closest;

        #endregion
    }

    private void UpdateAimPoint()
    {
        #region Aim Point

        currentAimPoint = null;
        if (currentTarget == null) return;

        foreach (Transform t in currentTarget.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag(aimChildTag))
            {
                currentAimPoint = t;
                return;
            }
        }

        currentAimPoint = currentTarget;

        #endregion
    }

    private void RotateTurret()
    {
        #region Rotation

        if (currentAimPoint != null)
        {
            Vector3 dir = (currentAimPoint.position - turretHead.position).normalized;
            Quaternion lookRot = Quaternion.LookRotation(dir);
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion defaultRot = Quaternion.Euler(defaultLocalEuler);
            turretHead.localRotation = Quaternion.Slerp(turretHead.localRotation, defaultRot, returnSpeed * Time.deltaTime);
        }

        #endregion
    }

    private void HandleShooting()
    {
        #region Shooting Switch

        if (currentAimPoint == null) return;

        switch (currentMode)
        {
            case GunMode.Rifle: Rifle(); break;
            case GunMode.MachineGun: MachineGun(); break;
            case GunMode.Shotgun: Shotgun(); break;
            case GunMode.Cannon: Cannon(); break;
        }

        #endregion
    }

    private void Rifle()
    {
        #region Rifle

        rifleTimer += Time.deltaTime;
        if (rifleTimer >= rifleFireRate)
        {
            rifleTimer = 0f;
            Fire(rifleProjectile, 0f);
        }

        #endregion
    }

    private void MachineGun()
    {
        #region Machine Gun

        machineTimer += Time.deltaTime;
        if (machineTimer >= machineFireRate)
        {
            machineTimer = 0f;
            FireMachineGun();
        }

        #endregion
    }

    private void Shotgun()
    {
        #region Shotgun (FIXED)

        shotgunTimer += Time.deltaTime;

        if (shotgunTimer >= shotgunCooldown)
        {
            shotgunTimer = 0f;
            FireShotgun();
        }

        #endregion
    }

    private void FireShotgun()
    {
        #region Fire Shotgun (Instant Burst, Flat Cone)

        if (shotgunProjectile == null || muzzlePoint == null || currentAimPoint == null) return;

        Vector3 baseDirection = (currentAimPoint.position - muzzlePoint.position).normalized;

        float halfAngle = detectionAngle * 0.5f;

        for (int i = 0; i < shotgunPellets; i++)
        {
            // Horizontal rotation ONLY (Y axis)
            float randomAngle = Random.Range(-halfAngle, halfAngle);
            Quaternion spreadRot = Quaternion.Euler(0f, randomAngle, 0f);

            Vector3 direction = spreadRot * baseDirection;

            GameObject projectile = Instantiate(
                shotgunProjectile,
                muzzlePoint.position,
                Quaternion.LookRotation(direction)
            );

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = direction * projectileSpeed;

            Projectile proj = projectile.GetComponent<Projectile>();
            if (proj != null) proj.attacker = transform.root.gameObject;
        }

        #endregion
    }

    private void Cannon()
    {
        #region Cannon

        cannonTimer += Time.deltaTime;
        if (cannonTimer >= cannonCooldown)
        {
            cannonTimer = 0f;
            Fire(cannonProjectile, 0f);
        }

        #endregion
    }

    private void FireMachineGun()
    {
        #region Machine Gun Fire

        if (machineProjectile == null || muzzlePoint == null || currentAimPoint == null) return;

        Vector3 direction = (currentAimPoint.position - muzzlePoint.position).normalized;

        Vector3 horizontalSpread = new Vector3(
            Random.Range(-machineSpread, machineSpread) * 0.01f,
            0f,
            Random.Range(-machineSpread, machineSpread) * 0.01f
        );

        direction += horizontalSpread;
        direction.Normalize();

        GameObject projectile = Instantiate(machineProjectile, muzzlePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = direction * projectileSpeed;

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null) proj.attacker = transform.root.gameObject;

        #endregion
    }

    private void Fire(GameObject prefab, float spread)
    {
        #region Generic Fire

        if (prefab == null || muzzlePoint == null || currentAimPoint == null) return;

        Vector3 direction = (currentAimPoint.position - muzzlePoint.position).normalized;

        direction += new Vector3(
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f
        );

        direction.Normalize();

        GameObject projectile = Instantiate(prefab, muzzlePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = direction * projectileSpeed;

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null) proj.attacker = transform.root.gameObject;

        #endregion
    }
}