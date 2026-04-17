using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Damage Settings

    public float damage = 10f;

    // NEW: Random damage range offsets
    [SerializeField] private float minDamageOffset = -5f;
    [SerializeField] private float maxDamageOffset = 5f;

    #endregion

    #region Lifetime

    [SerializeField] private float lifeTime = 5f;
    [HideInInspector] public GameObject attacker;

    #endregion

    #region Layers

    [Header("Layers")]
    public LayerMask targetLayer;
    public LayerMask sacredLayer;

    #endregion

    #region State

    private enum ProjectileState
    {
        Normal,
        PassedSacred
    }

    private ProjectileState _state = ProjectileState.Normal;
    private bool _destroyed = false;

    #endregion


    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_destroyed) return;

        int otherLayerBit = 1 << other.gameObject.layer;
        bool isTarget = (targetLayer.value & otherLayerBit) != 0;
        bool isSacred = (sacredLayer.value & otherLayerBit) != 0;

        #region Sacred Handling

        if (isSacred)
        {
            _state = ProjectileState.PassedSacred;
            return;
        }

        #endregion

        #region Target Handling

        if (isTarget)
        {
            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                float finalDamage = GetRandomDamage();
                health.TakeDamage(finalDamage, attacker);
            }

            DestroyProjectile();
            return;
        }

        #endregion

        #region Default Collision

        DestroyProjectile();

        #endregion
    }


    private float GetRandomDamage()
    {
        #region Random Damage Calculation

        float randomOffset = Random.Range(minDamageOffset, maxDamageOffset);
        float finalDamage = damage + randomOffset;

        // Optional: prevent negative damage (unless you're into healing enemies 💀)
        return Mathf.Max(0f, finalDamage);

        #endregion
    }


    private void DestroyProjectile()
    {
        _destroyed = true;
        Destroy(gameObject);
    }
}