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
        Debug.Log($"[Projectile] Spawned: {gameObject.name}");
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Guard against multiple collisions firing after Destroy() is called
        if (_destroyed) return;

        int otherLayerBit = 1 << other.gameObject.layer;
        bool isTarget = (targetLayer.value & otherLayerBit) != 0;
        bool isSacred = (sacredLayer.value & otherLayerBit) != 0;

        Debug.Log($"[Projectile] Hit: {other.gameObject.name} | Layer: {LayerMask.LayerToName(other.gameObject.layer)} | State: {_state}");

        // Sacred layer never affects the projectile — just flag that we passed through it
        if (isSacred)
        {
            Debug.Log("[Projectile] Sacred layer passed through → state upgraded to PassedSacred");
            _state = ProjectileState.PassedSacred;
            return;
        }

        // Target layer → always deal damage and destroy, regardless of state
        if (isTarget)
        {
            Debug.Log("[Projectile] Target hit → dealing damage");
            Health health = other.GetComponent<Health>();
            if (health != null)
                health.TakeDamage(damage, attacker);
            else
                Debug.Log("[Projectile] No Health component found.");

            DestroyProjectile();
            return;
        }

        // Any other layer:
        //   Normal state       → destroy immediately
        //   PassedSacred state → destroy (sacred already let it pass, but now it hit something real)
        Debug.Log($"[Projectile] Non-sacred, non-target hit in state {_state} → projectile destroyed");
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        _destroyed = true;
        Destroy(gameObject);
    }
}