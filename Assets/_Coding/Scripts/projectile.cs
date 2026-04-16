using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    [SerializeField] private float lifeTime = 5f;
    [HideInInspector] public GameObject attacker;

    [Header("Layers")]
    public LayerMask targetLayer;
    public LayerMask sacredLayer;

    private enum ProjectileState
    {
        Normal,         // Default: hits target = damage, hits default = destroy
        PassedSacred    // Passed through sacred: hits target = damage, hits anything else = destroy
    }

    private ProjectileState _state = ProjectileState.Normal;
    private bool _destroyed = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Guard against multiple collisions firing after Destroy() is called
        if (_destroyed) return;

        int otherLayerBit = 1 << other.gameObject.layer;
        bool isTarget = (targetLayer.value & otherLayerBit) != 0;
        bool isSacred = (sacredLayer.value & otherLayerBit) != 0;


        // Sacred layer never affects the projectile — just flag that we passed through it
        if (isSacred)
        {
            _state = ProjectileState.PassedSacred;
            return;
        }

        // Target layer → always deal damage and destroy, regardless of state
        if (isTarget)
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage, attacker);
            else

            DestroyProjectile();
            return;
        }

        // Any other layer:
        //   Normal state       → destroy immediately
        //   PassedSacred state → destroy (sacred already let it pass, but now it hit something real)
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        _destroyed = true;
        Destroy(gameObject);
    }
}