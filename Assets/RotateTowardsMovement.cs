using UnityEngine;

public class RotateTowardsMovement : MonoBehaviour
{
    #region Settings
    [SerializeField] private float rotationSpeed = 10f;
    #endregion

    #region References
    [SerializeField] private Rigidbody rb;
    #endregion

    #region Unity Methods
    private void Reset()
    {
        #region Auto Assign Rigidbody
        rb = GetComponent<Rigidbody>();
        #endregion
    }

    private void FixedUpdate()
    {
        #region Rotate Towards Movement
        Rotate();
        #endregion
    }
    #endregion

    #region Core Logic
    private void Rotate()
    {
        #region Get Movement Direction

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;

        #endregion

        #region Check If Moving

        if (velocity.sqrMagnitude < 0.01f)
            return;

        #endregion

        #region Calculate Target Rotation

        Quaternion targetRotation = Quaternion.LookRotation(velocity);

        #endregion

        #region Smooth Rotate

        Quaternion smoothedRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(smoothedRotation);

        #endregion
    }
    #endregion
}