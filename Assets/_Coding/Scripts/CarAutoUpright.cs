using UnityEngine;

public class CarAutoUpright : MonoBehaviour
{
    #region Settings

    [Header("Detection")]

    [SerializeField] private float uprightDotThreshold = 0.3f;
    [SerializeField] private float timeBeforeUpright = 2f;

    #endregion


    #region Upright Settings

    [Header("Upright Behavior")]

    [SerializeField] private float uprightSpeed = 5f;

    // NEW: Force applied upward
    [SerializeField] private float liftForce = 10f;

    // Where the force is applied (local offset)
    [SerializeField] private Vector3 liftPointOffset = Vector3.zero;

    #endregion


    #region Runtime Values

    private float flipTimer = 0f;
    private bool isUprighting = false;

    private Rigidbody rb;

    #endregion


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        #region Update Logic

        CheckIfFlipped();

        if (isUprighting)
        {
            UprightCar();
        }

        #endregion
    }


    private void CheckIfFlipped()
    {
        #region Flip Detection

        float dot = Vector3.Dot(transform.up, Vector3.up);

        if (dot < uprightDotThreshold)
        {
            flipTimer += Time.deltaTime;

            if (flipTimer >= timeBeforeUpright)
            {
                StartUprighting();
            }
        }
        else
        {
            flipTimer = 0f;
            isUprighting = false;
        }

        #endregion
    }


    private void StartUprighting()
    {
        #region Start Uprighting

        isUprighting = true;

        // Stop spin madness
        rb.angularVelocity = Vector3.zero;

        #endregion
    }


    private void UprightCar()
    {
        #region Upright Logic

        // Apply upward force from a point below the car
        Vector3 liftPoint = transform.TransformPoint(liftPointOffset);

        rb.AddForceAtPosition(Vector3.up * liftForce, liftPoint, ForceMode.Force);

        // Keep forward direction but remove tilt
        Vector3 forward = transform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.01f)
            forward = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);

        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * uprightSpeed));

        // Stop when upright
        float dot = Vector3.Dot(transform.up, Vector3.up);
        if (dot > 0.95f)
        {
            isUprighting = false;
            flipTimer = 0f;
        }

        #endregion
    }


    private void OnDrawGizmosSelected()
    {
        #region Gizmos

        Gizmos.color = Color.green;

        Vector3 liftPoint = transform.TransformPoint(liftPointOffset);
        Gizmos.DrawSphere(liftPoint, 0.2f);

        #endregion
    }
}