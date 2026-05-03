using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrapZone : MonoBehaviour
{
    [SerializeField] private float teleportThreshold = 50f;
    [SerializeField] private float gizmosSize = 0.5f;
    [SerializeField] private bool turnOnGizmos = true;

    private void Update()
    {
        foreach (GameObject enemy in EnemiesList.instance.enemyList)
        {
            if (enemy.transform.position.x > transform.position.x + teleportThreshold)
            {
                enemy.transform.position += new Vector3(-teleportThreshold * 2f, 0f, 0f);
                enemy.transform.LookAt(PlayerInstance.instance.transform.position);
            }
            if (enemy.transform.position.x < transform.position.x - teleportThreshold)
            {
                enemy.transform.position += new Vector3(teleportThreshold * 2f, 0f, 0f);
                enemy.transform.LookAt(PlayerInstance.instance.transform.position);
            }
            if (enemy.transform.position.z > transform.position.z + teleportThreshold)
            {
                enemy.transform.position += new Vector3(0f, 0f, -teleportThreshold * 2f);
                enemy.transform.LookAt(PlayerInstance.instance.transform.position);
            }
            if (enemy.transform.position.z < transform.position.z - teleportThreshold)
            {
                enemy.transform.position += new Vector3(0f, 0f, teleportThreshold * 2f);
                enemy.transform.LookAt(PlayerInstance.instance.transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!turnOnGizmos) return;

        Gizmos.color = Color.red;   

        Vector3 rightDotPosition = transform.position + new Vector3(teleportThreshold, 0f, 0f);
        Vector3 leftDotPosition = transform.position + new Vector3(-teleportThreshold, 0f, 0f);
        Vector3 topDotPosition = transform.position + new Vector3(0f, 0f, teleportThreshold);
        Vector3 bottomDotPosition = transform.position + new Vector3(0f, 0f, -teleportThreshold);

        Gizmos.DrawSphere(rightDotPosition, gizmosSize);
        Gizmos.DrawSphere(leftDotPosition, gizmosSize);
        Gizmos.DrawSphere(topDotPosition, gizmosSize);
        Gizmos.DrawSphere(bottomDotPosition, gizmosSize);
    }
}