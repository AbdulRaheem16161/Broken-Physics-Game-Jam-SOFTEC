using System.Collections;
using UnityEngine;

public class MCarRoofTurretGun : MonoBehaviour
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
    [SerializeField] private float detectionRange = 25f;
    [SerializeField] [Range(1f, 360f)] private float detectionAngle = 90f;
    [SerializeField] private float pointBlankRange = 6f;
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

    [Header("🔊 Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private float baseVolume = 1f;
    [SerializeField] private Vector2 pitchVariation = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;

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

    private void Awake()
    {
        #region Audio Setup

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // FULL 3D SOUND (important)
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 60f;
        audioSource.playOnAwake = false;

        #endregion
    }

    private void Update()
    {
        FindClosestTargetInCone();
        UpdateAimPoint();
        RotateTurret();
        HandleShooting();
    }

    private void FindClosestTargetInCone()
    {
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
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRot, rotationSpeed * Time.deltaTime);
        }
        else
        {
            Quaternion defaultRot = Quaternion.Euler(defaultLocalEuler);
            turretHead.localRotation = Quaternion.Slerp(turretHead.localRotation, defaultRot, returnSpeed * Time.deltaTime);
        }
    }

    private void HandleShooting()
    {
        if (currentAimPoint == null) return;

        switch (currentMode)
        {
            case GunMode.Rifle: Rifle(); break;
            case GunMode.MachineGun: MachineGun(); break;
            case GunMode.Shotgun: Shotgun(); break;
            case GunMode.Cannon: Cannon(); break;
        }
    }

    private void PlayShootSound()
    {
        #region 3D Sound Playback

        if (shootClip == null || audioSource == null) return;

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(shootClip, baseVolume);

        #endregion
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
            FireMachineGun();
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

    private void FireMachineGun()
    {
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

        PlayShootSound();
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

        direction.Normalize();

        GameObject projectile = Instantiate(prefab, muzzlePoint.position, Quaternion.LookRotation(direction));

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = direction * projectileSpeed;

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null) proj.attacker = transform.root.gameObject;

        PlayShootSound();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;
        float halfAngle = detectionAngle * 0.5f;

        Gizmos.color = new Color(1f, 0.4f, 0f, 0.15f);

        int segments = 40;
        float angleStep = detectionAngle / segments;
        float startAngle = -halfAngle;

        Vector3 prevPoint = origin + Quaternion.Euler(0f, startAngle, 0f) * forward * detectionRange;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector3 nextPoint = origin + Quaternion.Euler(0f, currentAngle, 0f) * forward * detectionRange;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        Gizmos.color = new Color(1f, 0.4f, 0f, 0.6f);

        Vector3 leftBound  = Quaternion.Euler(0f, -halfAngle, 0f) * forward * detectionRange;
        Vector3 rightBound = Quaternion.Euler(0f,  halfAngle, 0f) * forward * detectionRange;

        Gizmos.DrawLine(origin, origin + leftBound);
        Gizmos.DrawLine(origin, origin + rightBound);

        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(origin, pointBlankRange);

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(origin, currentTarget.position);
            Gizmos.DrawWireSphere(currentTarget.position, 0.4f);
        }
    }
}