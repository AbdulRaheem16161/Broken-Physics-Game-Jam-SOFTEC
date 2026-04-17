using UnityEngine;
using System.Collections;

public class IdleTeleportHandler : MonoBehaviour
{
    #region Settings

    [Header("Target Object")]
    [SerializeField] private Transform target;

    [Header("Afterimage Prefab")]
    [SerializeField] private GameObject afterImagePrefab;

    [Header("Movement Settings")]
    [SerializeField] private float movementThreshold = 0.01f;
    [SerializeField] private float idleTimeRequired = 3f;

    [Header("Afterimage Lifetime")]
    [SerializeField] private float afterImageLifetime = 2f;

    [Header("Teleport Area")]
    [SerializeField] private Vector3 areaCenter;
    [SerializeField] private Vector2 areaSize = new Vector2(10f, 10f);

    #endregion

    #region Runtime Values

    private Vector3 lastPosition;
    private float idleTimer;

    #endregion

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
        #region Update Loop

        if (target == null) return;

        CheckMovement();

        #endregion
    }

    private void CheckMovement()
    {
        #region Movement Check

        float distance = Vector3.Distance(target.position, lastPosition);

        if (distance < movementThreshold)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeRequired)
            {
                TriggerTeleport();
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
        }

        lastPosition = target.position;

        #endregion
    }

    private void TriggerTeleport()
    {
        #region Teleport Logic

        // 1. Instantiate afterimage
        GameObject afterImage = Instantiate(afterImagePrefab);

        // 2. Parent to target
        afterImage.transform.SetParent(target);

        // 3. Reset local position (perfect alignment)
        afterImage.transform.localPosition = Vector3.zero;
        afterImage.transform.localRotation = Quaternion.identity;

        // 4. Unparent (keep world position)
        afterImage.transform.SetParent(null);

        // 5. Teleport target
        target.position = GetRandomPosition();

        // 6. Destroy afterimage after delay
        StartCoroutine(DestroyAfterTime(afterImage, afterImageLifetime));

        #endregion
    }

    private Vector3 GetRandomPosition()
    {
        #region Random Position

        float randomX = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float randomZ = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

        return new Vector3(
            areaCenter.x + randomX,
            target.position.y,
            areaCenter.z + randomZ
        );

        #endregion
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        #region Destroy Coroutine

        yield return new WaitForSeconds(time);

        if (obj != null)
        {
            Destroy(obj);
        }

        #endregion
    }

    private void OnDrawGizmosSelected()
    {
        #region Gizmos

        Gizmos.color = Color.cyan;

        Vector3 size = new Vector3(areaSize.x, 0.1f, areaSize.y);
        Gizmos.DrawWireCube(areaCenter, size);

        #endregion
    }
}