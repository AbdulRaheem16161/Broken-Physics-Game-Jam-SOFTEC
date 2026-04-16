using UnityEngine;
using ArcadeVP;

public class PlayerAirTimeFix : MonoBehaviour
{
    #region References
    [SerializeField] private ArcadeVehicleController controller;
    [SerializeField] private Rigidbody rb;
    #endregion

    #region Settings
    [SerializeField] private float airControlStrength = 1f;
    #endregion

    private void Awake()
    {
        #region Auto Assign

        if (controller == null)
            controller = GetComponent<ArcadeVehicleController>();

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        #endregion
    }

    private void FixedUpdate()
    {
        #region Air Fix (SAFE)

        if (!controller.grounded())
        {
            // Counteract ONLY extra slow-feel from gravity
            Vector3 extraGravity = Physics.gravity * (1f - Time.timeScale);

            rb.AddForce(extraGravity, ForceMode.Acceleration);
        }

        #endregion
    }
}