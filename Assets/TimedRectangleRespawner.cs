using UnityEngine;

public class RectIntervalTeleporter : MonoBehaviour
{
    #region Settings

    [Header("Target To Teleport")]
    [SerializeField] private Transform target;

    [Header("Teleport Rectangle (Local Space of This Object)")]
    [SerializeField] private Vector2 size = new Vector2(5f, 5f);

    [Header("Timing")]
    [SerializeField] private float interval = 2f;

    #endregion

    #region Runtime Values

    private float timer;

    #endregion

    #region Unity Methods

    private void Update()
    {
        #region Timer Logic

        if (target == null) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            TeleportTarget();
            timer = 0f;
        }

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