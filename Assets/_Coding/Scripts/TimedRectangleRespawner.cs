using UnityEngine;

public class RectIntervalTeleporter : MonoBehaviour
{
    #region Settings

    [Header("Target To Teleport")]
    [SerializeField] private Transform target;

    [Header("Teleport Rectangle (World Space)")]
    [SerializeField] private Vector2 size = new Vector2(5f, 5f);

    [Header("Timing")]
    [SerializeField] private float interval = 2f;

    [Header("Movement Detection")]
    [SerializeField] private float movementThreshold = 0.01f;

    #endregion

    #region Runtime Values

    private float timer;
    private Vector3 lastPosition;

    [Header("Debug")]
    [SerializeField, Tooltip("Current movement speed")]
    private float currentVelocity;

    #endregion

    #region Unity Methods

    private void Start()
    {
        #region Initialization

        if (target != null)
        {
            lastPosition = target.position;
        }

        #endregion
    }

    private void Update()
    {
        #region Update Logic

        if (target == null) return;

        // Calculate velocity (units per second)
        float distance = Vector3.Distance(target.position, lastPosition);
        currentVelocity = distance / Time.deltaTime;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            if (!IsMoving())
            {
                TeleportTarget();
            }

            timer = 0f;
        }

        lastPosition = target.position;

        #endregion
    }

    private void OnDrawGizmos()
    {
        #region Draw Rectangle Gizmo

        Gizmos.color = Color.cyan;

        Vector3 center = transform.position;

        Vector3 topLeft     = center + new Vector3(-size.x / 2f, 0f,  size.y / 2f);
        Vector3 topRight    = center + new Vector3( size.x / 2f, 0f,  size.y / 2f);
        Vector3 bottomLeft  = center + new Vector3(-size.x / 2f, 0f, -size.y / 2f);
        Vector3 bottomRight = center + new Vector3( size.x / 2f, 0f, -size.y / 2f);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);

        #endregion
    }

    #endregion

    #region Movement Check

    private bool IsMoving()
    {
        #region Movement Detection

        return currentVelocity > movementThreshold;

        #endregion
    }

    #endregion

    #region Teleport Logic

    private void TeleportTarget()
    {
        #region Random Position Calculation

        Vector3 center = transform.position;

        float randomX = Random.Range(-size.x / 2f, size.x / 2f);
        float randomZ = Random.Range(-size.y / 2f, size.y / 2f);

        Vector3 newPos = new Vector3(
            center.x + randomX,
            target.position.y,
            center.z + randomZ
        );

        target.position = newPos;

        #endregion
    }

    #endregion
}