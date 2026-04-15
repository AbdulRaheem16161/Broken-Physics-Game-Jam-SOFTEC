using UnityEngine;

public class LockToReference : MonoBehaviour
{
    [Header("Reference Point")]
    public Transform referencePoint;

    private void LateUpdate()
    {
        if (referencePoint == null) return;

        // Force exact local position alignment
        transform.position = referencePoint.position;

        // Optional: match rotation too (uncomment if needed)
        // transform.rotation = referencePoint.rotation;
    }
}