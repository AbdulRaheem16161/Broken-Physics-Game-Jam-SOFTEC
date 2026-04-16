using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    #region Settings

    [Header("Detection")]
    // Radius within which the player triggers the scene change
    [SerializeField] private float radius = 5f;

    [Header("Scene")]
    // Name of the scene to load next (must be added in Build Settings)
    [SerializeField] private string nextSceneName;

    [Header("Delay")]
    // Delay before scene change after player enters radius
    [SerializeField] private float delayBeforeChange = 2f;

    #endregion

    #region Runtime Values

    // Prevent multiple triggers
    private bool isTriggered = false;

    // Cached player transform
    private Transform player;

    #endregion

    #region Unity Events

    private void Start()
    {
        #region Find Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("SceneChanger: No object with tag 'Player' found in scene.");
        }
        #endregion
    }

    private void Update()
    {
        #region Radius Check
        if (isTriggered || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= radius)
        {
            StartCoroutine(ChangeSceneAfterDelay());
        }
        #endregion
    }

    #endregion

    #region Scene Change Logic

    private IEnumerator ChangeSceneAfterDelay()
    {
        #region Trigger Guard
        isTriggered = true;
        #endregion

        #region Wait Before Scene Load
        yield return new WaitForSeconds(delayBeforeChange);
        #endregion

        #region Load Scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("SceneChanger: Next scene name is empty.");
        }
        #endregion
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        #region Draw Radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
        #endregion
    }

    #endregion
}