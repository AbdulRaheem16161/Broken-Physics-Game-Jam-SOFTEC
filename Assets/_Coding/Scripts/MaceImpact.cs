using UnityEngine;

public class MaceImpact : MonoBehaviour
{
    [Header("Detection")]
    public float radius = 2f;
    public LayerMask hitLayers;

    [Header("Damage Settings")]
    public LayerMask targetLayer;
    public float damage = 20f;

    [Header("Impact Force")]
    public float bounceForce = 10f;
    public float upwardBoost = 2f;

    private void Update()
    {
        DetectAndHit();
    }

    private void DetectAndHit()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, hitLayers);

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;

            Vector3 dir = (hit.transform.position - transform.position).normalized;
            Vector3 force = dir * bounceForce + Vector3.up * upwardBoost;

            // 1. ALWAYS apply bounce
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }

            // 2. ONLY apply damage if target layer matches
            int hitLayerBit = 1 << hit.gameObject.layer;
            bool isTarget = (targetLayer.value & hitLayerBit) != 0;

            if (isTarget)
            {
                Health health = hit.GetComponent<Health>();

                if (health != null)
                {
                    health.TakeDamage(damage, gameObject);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}