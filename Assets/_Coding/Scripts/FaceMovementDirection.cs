using UnityEngine;

public class FaceMovementDirection_NoRB : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSmoothness = 5f;
    public float minMoveThreshold = 0.001f;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Calculate movement delta
        Vector3 movement = transform.position - lastPosition;

        // Ignore tiny movement (prevents jitter)
        if (movement.sqrMagnitude > minMoveThreshold)
        {
            Vector3 direction = movement.normalized;

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothness * Time.deltaTime
            );
        }

        // Update last position
        lastPosition = transform.position;
    }
}