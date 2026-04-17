using UnityEngine;
using System.Collections.Generic;

public class MaceImpact : MonoBehaviour
{
    #region Enums

    public enum Shape
    {
        Sphere,
        Rectangle
    }

    #endregion

    #region Detection Settings

    [Header("Detection Shape")]
    [SerializeField] private Shape shape;

    [SerializeField] private float radius = 2f;
    [SerializeField] private Vector3 boxSize = new Vector3(2f, 2f, 2f);
    [SerializeField] private Vector3 offset;
    [SerializeField] private LayerMask hitLayers;

    #endregion

    #region Damage Settings

    [Header("Damage Settings")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float damage = 20f;
    [SerializeField] private float damageCooldown = 0.5f;

    #endregion

    #region Impact Force

    [Header("Impact Force")]
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private float upwardBoost = 2f;

    #endregion

    #region Runtime Values

    private Dictionary<GameObject, float> lastHitTime = new Dictionary<GameObject, float>();

    #endregion

    #region Unity Methods

    private void Update()
    {
        DetectAndHit();
    }

    #endregion

    #region Detection Logic

    private void DetectAndHit()
    {
        Collider[] hits = GetHits();

        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;

            if (rb != null)
            {
                #region LAYER MATRIX CHECK (IMPORTANT FIX)

                int selfLayer = gameObject.layer;
                int otherLayer = hit.gameObject.layer;

                // If Unity says these layers should NOT collide → skip bounce
                if (Physics.GetIgnoreLayerCollision(selfLayer, otherLayer))
                    continue;

                #endregion

                #region Force Calculation

                Vector3 dir = (hit.transform.position - GetCenter()).normalized;
                Vector3 force = dir * bounceForce + Vector3.up * upwardBoost;

                rb.AddForce(force, ForceMode.Impulse);

                #endregion
            }

            #region Damage Logic (unchanged)

            int hitLayerBit = 1 << hit.gameObject.layer;
            bool isTarget = (targetLayer.value & hitLayerBit) != 0;

            if (isTarget)
            {
                GameObject target = hit.gameObject;

                if (CanDamage(target))
                {
                    Health health = target.GetComponent<Health>();

                    if (health != null)
                    {
                        health.TakeDamage(damage, gameObject);
                        lastHitTime[target] = Time.time;
                    }
                }
            }

            #endregion
        }
    }

    #endregion

    #region Cooldown

    private bool CanDamage(GameObject target)
    {
        if (!lastHitTime.ContainsKey(target))
            return true;

        return Time.time >= lastHitTime[target] + damageCooldown;
    }

    #endregion

    #region Detection

    private Collider[] GetHits()
    {
        Vector3 center = GetCenter();

        switch (shape)
        {
            case Shape.Sphere:
                return Physics.OverlapSphere(center, radius, hitLayers);

            case Shape.Rectangle:
                return Physics.OverlapBox(center, boxSize / 2f, transform.rotation, hitLayers);

            default:
                return new Collider[0];
        }
    }

    private Vector3 GetCenter()
    {
        return transform.position + transform.TransformDirection(offset);
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 center = GetCenter();

        switch (shape)
        {
            case Shape.Sphere:
                Gizmos.DrawWireSphere(center, radius);
                break;

            case Shape.Rectangle:
                Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, boxSize);
                break;
        }
    }

    #endregion
}