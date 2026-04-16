using UnityEngine;

/// <summary>
/// Clamps a game object's X and Y rotation to specified limits.
/// Provides visual feedback via gizmos showing the allowed rotation boundaries.
/// </summary>
public class RotationCapper : MonoBehaviour
{
    [Header("Rotation Limits")]
    [SerializeField] private Vector2 xRotationLimits = new Vector2(-45f, 45f);
    [SerializeField] private Vector2 yRotationLimits = new Vector2(-90f, 90f);
    
    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0.5f, 0.7f);
    [SerializeField] private float gizmoSize = 1f;

    private Vector3 previousRotation = Vector3.zero;

    private void Start()
    {
        previousRotation = transform.eulerAngles;
    }

    private void Update()
    {
        ClampRotation();
    }

    /// <summary>
    /// Clamps the object's X and Y rotation to the specified limits.
    /// Z rotation is left unclamped.
    /// </summary>
    private void ClampRotation()
    {
        Vector3 currentRotation = transform.eulerAngles;

        // Normalize angles to -180 to 180 range for easier comparison
        float x = NormalizeAngle(currentRotation.x);
        float y = NormalizeAngle(currentRotation.y);

        // Clamp X and Y rotation
        x = Mathf.Clamp(x, xRotationLimits.x, xRotationLimits.y);
        y = Mathf.Clamp(y, yRotationLimits.x, yRotationLimits.y);

        // Apply clamped rotation (Z remains unchanged)
        transform.eulerAngles = new Vector3(x, y, currentRotation.z);
    }

    /// <summary>
    /// Normalizes an angle to the range of -180 to 180 degrees.
    /// </summary>
    private float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        if (angle > 180f)
            angle -= 360f;
        else if (angle < -180f)
            angle += 360f;
        return angle;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Gizmos.color = gizmoColor;
        DrawRotationBounds();
    }

    /// <summary>
    /// Visualizes the rotation limits as wireframe boxes and constraint lines.
    /// </summary>
    private void DrawRotationBounds()
    {
        Vector3 position = transform.position;
        float scale = gizmoSize;

        // Draw X rotation constraint (pitch) - Red lines
        Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
        DrawRotationArc(position, Vector3.right, xRotationLimits, scale);

        // Draw Y rotation constraint (yaw) - Green lines
        Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
        DrawRotationArc(position, Vector3.up, yRotationLimits, scale);

        // Draw Z axis reference - Blue line
        Gizmos.color = new Color(0f, 0f, 1f, 0.4f);
        Gizmos.DrawLine(position, position + transform.forward * scale);

        // Draw center sphere
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(position, scale * 0.1f);
    }

    /// <summary>
    /// Draws an arc representing rotation constraints around a given axis.
    /// </summary>
    private void DrawRotationArc(Vector3 center, Vector3 axis, Vector2 limits, float radius)
    {
        int segments = 16;
        Vector3 lastPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(limits.x, limits.y, i / (float)segments);
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            
            // Create a perpendicular vector to rotate around the axis
            Vector3 perpendicular = axis == Vector3.right ? Vector3.forward : 
                                    axis == Vector3.up ? Vector3.right : 
                                    Vector3.up;
            
            Vector3 point = center + rotation * perpendicular * radius;

            if (i > 0)
            {
                Gizmos.DrawLine(lastPoint, point);
            }

            lastPoint = point;
        }

        // Draw limit indicator lines
        Quaternion minRotation = Quaternion.AngleAxis(limits.x, axis);
        Quaternion maxRotation = Quaternion.AngleAxis(limits.y, axis);
        Vector3 perpendic = axis == Vector3.right ? Vector3.forward : 
                           axis == Vector3.up ? Vector3.right : 
                           Vector3.up;

        Vector3 minPoint = center + minRotation * perpendic * radius;
        Vector3 maxPoint = center + maxRotation * perpendic * radius;

        Gizmos.DrawLine(center, minPoint);
        Gizmos.DrawLine(center, maxPoint);
    }

    // Public methods for runtime adjustment
    public void SetXRotationLimits(float min, float max)
    {
        xRotationLimits = new Vector2(Mathf.Min(min, max), Mathf.Max(min, max));
    }

    public void SetYRotationLimits(float min, float max)
    {
        yRotationLimits = new Vector2(Mathf.Min(min, max), Mathf.Max(min, max));
    }

    public void SetGizmosEnabled(bool enabled)
    {
        showGizmos = enabled;
    }

    public Vector2 GetXRotationLimits() => xRotationLimits;
    public Vector2 GetYRotationLimits() => yRotationLimits;
    public bool GetGizmosEnabled() => showGizmos;
}