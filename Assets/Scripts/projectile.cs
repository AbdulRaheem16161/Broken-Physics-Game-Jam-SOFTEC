using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    [SerializeField] private float lifeTime = 5f;

    [HideInInspector] public GameObject attacker;

    [Header("Layers")]
    public LayerMask targetLayer;
    public LayerMask sacredLayer;

    private void Start()
    {
        Debug.Log($"[Projectile] Spawned: {gameObject.name}");
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Projectile] Hit: {other.gameObject.name}");
        Debug.Log($"[Projectile] Hit Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        int otherLayerBit = 1 << other.gameObject.layer;

        bool isTarget = (targetLayer.value & otherLayerBit) != 0;
        bool isSacred = (sacredLayer.value & otherLayerBit) != 0;

        // 1. TARGET LAYER → damage + destroy
        if (isTarget)
        {
            Debug.Log("[Projectile] Target hit → dealing damage");

            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage, attacker);
            }
            else
            {
                Debug.Log("[Projectile] No Health component found.");
            }

            Destroy(gameObject);
            return;
        }

        // 2. SACRED LAYER → do nothing
        if (isSacred)
        {
            Debug.Log("[Projectile] Sacred object hit → ignoring completely");
            return;
        }

        // 3. EVERYTHING ELSE → destroy projectile
        Debug.Log("[Projectile] Non-valid collision → projectile destroyed");
        Destroy(gameObject);
    }
}