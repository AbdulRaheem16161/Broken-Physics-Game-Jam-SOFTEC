using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(EnemiesList))]
public class GroundWrapZone : MonoBehaviour
{
    public GameObject testEnemy;

    public float teleportThreshold = 50f;
    public float gizmosSize = 0.5f;

    private void Update()
    {
        if (testEnemy.transform.position.x > transform.position.x + teleportThreshold)
        {
            testEnemy.transform.position += new Vector3(-teleportThreshold * 2f, 0f, 0f);
            testEnemy.transform.LookAt(PlayerManager.instance.transform.position);
        }
        if (testEnemy.transform.position.x < transform.position.x - teleportThreshold)
        {
            testEnemy.transform.position += new Vector3(teleportThreshold * 2f, 0f, 0f);
            testEnemy.transform.LookAt(PlayerManager.instance.transform.position);
        }
        if (testEnemy.transform.position.z > transform.position.z + teleportThreshold)
        {
            testEnemy.transform.position += new Vector3(0f, 0f, -teleportThreshold * 2f);
            testEnemy.transform.LookAt(PlayerManager.instance.transform.position);
        }
        if (testEnemy.transform.position.z < transform.position.z - teleportThreshold)
        {
            testEnemy.transform.position += new Vector3(0f, 0f, teleportThreshold * 2f);
            testEnemy.transform.LookAt(PlayerManager.instance.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
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