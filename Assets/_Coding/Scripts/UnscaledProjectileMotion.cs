using UnityEngine;

public class UnscaledProjectileMotion : MonoBehaviour
{
    #region Settings

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 5f;

    [Header("Slow Motion Modifier")]
    [SerializeField] private float timeSlowMultiplier = 1f;

    #endregion

    #region Runtime

    private Rigidbody rb;
    private float timer;

    private Vector3 storedVelocity;

    #endregion

    private void Awake()
    {
        #region Cache Rigidbody

        rb = GetComponent<Rigidbody>();

        #endregion
    }

    private void Update()
    {
        #region Lifetime (Unscaled Time)

        timer += Time.unscaledDeltaTime;

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }

        #endregion

        #region Apply Slow Motion Adjustment

        ApplyTimeModifier();

        #endregion
    }

    public void SetVelocity(Vector3 velocity)
    {
        #region Store Velocity

        storedVelocity = velocity;

        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }

        #endregion
    }

    private void ApplyTimeModifier()
    {
        #region Modify Speed Based on TimeScale

        if (rb == null) return;

        float timeScale = Time.timeScale;

        // If time is slowed, apply custom multiplier
        if (timeScale < 0.95f)
        {
            Vector3 modifiedVelocity = storedVelocity * timeSlowMultiplier;
            rb.linearVelocity = modifiedVelocity;
        }
        else
        {
            rb.linearVelocity = storedVelocity;
        }

        #endregion
    }
}