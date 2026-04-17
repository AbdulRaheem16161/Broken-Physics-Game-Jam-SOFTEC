using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    #region Settings

    [Header("Detection")]
    [SerializeField] private float radius = 5f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    [Header("Delay")]
    [SerializeField] private float delayBeforeChange = 2f;

    #endregion

    #region Audio

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Clips")]
    [SerializeField] private AudioClip loopClip;
    [SerializeField] private AudioClip finalClip;

    #endregion

    #region Runtime

    private Transform player;
    private bool isTriggered = false;

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
            Debug.LogWarning("SceneChanger: Player not found.");
        }
        #endregion

        #region Start Loop Sound
        if (audioSource != null && loopClip != null)
        {
            audioSource.clip = loopClip;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.Play();
        }
        #endregion
    }

    private void Update()
    {
        #region Safety Check
        if (isTriggered || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        HandleLoopVolume(distance);

        if (distance <= radius)
        {
            StartCoroutine(ChangeScene());
        }
        #endregion
    }

    #endregion

    #region Loop Sound Control

    private void HandleLoopVolume(float distance)
    {
        if (audioSource == null) return;

        float t = Mathf.Clamp01(1f - (distance / (radius * 2f)));
        audioSource.volume = t;
    }

    #endregion

    #region Scene Change

    private IEnumerator ChangeScene()
    {
        #region Lock Trigger
        isTriggered = true;
        #endregion

        #region Play Final Sound
        if (audioSource != null && finalClip != null)
        {
            audioSource.PlayOneShot(finalClip);
        }
        #endregion

        #region Wait
        yield return new WaitForSeconds(delayBeforeChange);
        #endregion

        #region Load Scene
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        #endregion
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    #endregion
}