using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWrapZone : MonoBehaviour
{
    [Header("Ground Size")]
    public float width = 20f;
    public float length = 20f;

    [Header("Target Settings")]
    public string targetTag = "Enemy";

    [Header("Teleport Settings")]
    public float teleportDelay = 1f;

    [Header("Gizmos")]
    public bool showGizmos = true;

    public Color rectangleColor = Color.green;
    public Color rightEdgeColor = Color.red;

    private HashSet<GameObject> processingObjects = new HashSet<GameObject>();

    private void Update()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject target in targets)
        {
            if (target == null)
                continue;

            Vector3 localPos = transform.InverseTransformPoint(target.transform.position);

            float rightEdge = width * 0.5f;

            // Check if object crossed RIGHT side
            if (localPos.x > rightEdge)
            {
                if (!processingObjects.Contains(target))
                {
                    StartCoroutine(TeleportToLeft(target));
                }
            }
        }
    }

    private IEnumerator TeleportToLeft(GameObject target)
    {
        processingObjects.Add(target);

        yield return new WaitForSeconds(teleportDelay);

        if (target != null)
        {
            Vector3 localPos = transform.InverseTransformPoint(target.transform.position);

            float leftEdge = -width * 0.5f;

            // Teleport to LEFT side
            localPos.x = leftEdge;

            target.transform.position = transform.TransformPoint(localPos);

            // Rotate 180 degrees on Y axis
            target.transform.Rotate(0f, 180f, 0f);
        }

        processingObjects.Remove(target);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
            return;

        Vector3 center = transform.position;

        // Draw flat rectangle
        Gizmos.color = rectangleColor;

        Vector3 size = new Vector3(width, 0.1f, length);

        Gizmos.DrawWireCube(center, size);

        // Draw RIGHT edge
        Gizmos.color = rightEdgeColor;

        Vector3 topRight = center + new Vector3(width * 0.5f, 0f, length * 0.5f);
        Vector3 bottomRight = center + new Vector3(width * 0.5f, 0f, -length * 0.5f);

        Gizmos.DrawLine(topRight, bottomRight);
    }
}