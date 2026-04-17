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

    #region Gun Mode

    [Header("Gun Mode")]
    [SerializeField] private GunMode currentMode;

    #endregion

    #region Targeting

    [Header("Targeting")]
    [SerializeField] private float detectionRange = 25f;

    [SerializeField, Range(1f, 360f)]
    private float detectionAngle = 90f;

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

    #region Projectile Settings

    [Header("Projectile Speed")]
    [SerializeField] private float projectileSpeed = 40f;

    [Header("Projectiles")]
    [SerializeField] private GameObject rifleProjectile;
    [SerializeField] private GameObject machineProjectile;
    [SerializeField] private GameObject shotgunProjectile;
    [SerializeField] private GameObject cannonProjectile;

    #endregion

    #region Audio

    [Header("Audio")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private float baseVolume = 1f;
    [SerializeField] private Vector2 pitchVariation = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;

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

    #region Runtime Targets

    private Transform currentTarget;
    private Transform currentAimPoint;

    #endregion

    private void Awake()
    {
        #region Audio Setup

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 60f;
        audioSource.playOnAwake = false;

        #endregion
    }

    private void Update()
    {
        #region Core Loop

        FindClosestTargetInCone();
        UpdateAimPoint();
        RotateTurret();
        HandleShooting();

        #endregion
    }

    #region Targeting

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
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dir);

                if (angle > detectionAngle * 0.5f)
                    continue;
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

    #endregion

    #region Rotation

    private void RotateTurret()
    {
        if (currentAimPoint != null)
        {
            Vector3 dir = (currentAimPoint.position - turretHead.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);

            turretHead.rotation = Quaternion.Slerp(
                turretHead.rotation,
                rot,
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

    #endregion

    #region Shooting

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

    private void PlayShootSound()
    {
        if (shootClip == null || audioSource == null) return;

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(shootClip, baseVolume);
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
            FireMachineGun();
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

    #region Core Fire Logic

    private void Fire(GameObject prefab, float spread)
    {
        if (prefab == null || muzzlePoint == null || currentAimPoint == null) return;

        Vector3 dir = (currentAimPoint.position - muzzlePoint.position).normalized;

        dir += new Vector3(
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f,
            Random.Range(-spread, spread) * 0.01f
        );

        dir.Normalize();

        GameObject obj = Instantiate(prefab, muzzlePoint.position, Quaternion.LookRotation(dir));

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;

        Projectile proj = obj.GetComponent<Projectile>();

        if (proj != null)
        {
            proj.attacker = transform.root.gameObject;

            var levelSystem = GetComponent<MCarRoofTurretGun_LevelSystem>();
            if (levelSystem != null)
            {
                proj.damage = levelSystem.GetCurrentDamage();
            }
        }

        PlayShootSound();
    }

    private void FireMachineGun()
    {
        if (machineProjectile == null || muzzlePoint == null || currentAimPoint == null) return;

        Vector3 dir = (currentAimPoint.position - muzzlePoint.position).normalized;

        dir += new Vector3(
            Random.Range(-machineSpread, machineSpread) * 0.01f,
            0f,
            Random.Range(-machineSpread, machineSpread) * 0.01f
        );

        dir.Normalize();

        GameObject obj = Instantiate(machineProjectile, muzzlePoint.position, Quaternion.LookRotation(dir));

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = dir * projectileSpeed;

        Projectile proj = obj.GetComponent<Projectile>();

        if (proj != null)
        {
            proj.attacker = transform.root.gameObject;

            var levelSystem = GetComponent<MCarRoofTurretGun_LevelSystem>();
            if (levelSystem != null)
            {
                proj.damage = levelSystem.GetCurrentDamage();
            }
        }

        PlayShootSound();
    }

    #endregion
}