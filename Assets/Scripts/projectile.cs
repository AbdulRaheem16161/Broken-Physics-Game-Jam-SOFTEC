using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    [SerializeField] private float lifeTime = 5f;

    [HideInInspector] public GameObject attacker;

    [Header("Allowed Target Layer")]
    public LayerMask targetLayer;

    private void Start()
    {
        Debug.Log($"[Projectile] Spawned: {gameObject.name}");
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Projectile] Hit: {other.gameObject.name}");
        Debug.Log($"[Projectile] Hit Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        // Check if hit object is in allowed target layer
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            Debug.Log("[Projectile] Valid target hit. Checking health...");

            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                Debug.Log("[Projectile] Damage applied!");
                health.TakeDamage(damage, attacker);
            }
            else
            {
                Debug.Log("[Projectile] No Health component found.");
            }
        }
        else
        {
            Debug.Log("[Projectile] Hit object NOT in target layer. No damage.");
        }

        Debug.Log("[Projectile] Destroying projectile.");
        Destroy(gameObject);
    }
}