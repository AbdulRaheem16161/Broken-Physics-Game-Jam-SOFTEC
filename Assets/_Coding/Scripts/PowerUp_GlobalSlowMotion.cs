using UnityEngine;

public class PowerUp_GlobalSlowMotion : MonoBehaviour, IPowerUp
{
      [SerializeField] private string displayName = "Slow Motion";

    public string DisplayName => displayName;

    #region References

    [Header("Slow Motion System")]
    [SerializeField] private GlobalSlowMotion slowMotion;

    #endregion

    private void Awake()
    {
        #region Auto Assign

        if (slowMotion == null)
        {
            slowMotion = GetComponent<GlobalSlowMotion>();
        }

        Debug.Log("[SlowMotion PowerUp] Awake -> Assigned slowMotion: " + (slowMotion != null));

        #endregion
    }

    #region IPowerUp

    public void ActivatePowerUp()
    {
        #region Activate Slow Motion

        Debug.Log("[SlowMotion PowerUp] ActivatePowerUp CALLED");

        if (slowMotion == null)
        {
            Debug.LogWarning("[SlowMotion PowerUp] Activate FAILED - slowMotion is NULL");
            return;
        }

        slowMotion.EnableSlowMotion();

        Debug.Log("[SlowMotion PowerUp] Slow Motion ENABLED");

        #endregion
    }

    public void DeactivatePowerUp()
    {
        #region Deactivate Slow Motion

        Debug.Log("[SlowMotion PowerUp] DeactivatePowerUp CALLED");

        if (slowMotion == null)
        {
            Debug.LogWarning("[SlowMotion PowerUp] Deactivate FAILED - slowMotion is NULL");
            return;
        }

        slowMotion.DisableSlowMotion();

        Debug.Log("[SlowMotion PowerUp] Slow Motion DISABLED");

        #endregion
    }

    #endregion
}