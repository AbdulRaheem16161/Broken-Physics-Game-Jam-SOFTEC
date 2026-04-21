using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoSceneLoader : MonoBehaviour
{
    #region References

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    #endregion

    #region Scene Settings

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName;

    [Header("Skip Settings")]
    [SerializeField] private KeyCode skipKey = KeyCode.T;

    [Header("Timer Settings")]
    [SerializeField] private float loadAfterSeconds = 0f; // 0 = disabled

    #endregion

    #region Runtime

    private float timer;
    private bool isLoading = false;

    #endregion

    private void Start()
    {
        #region Start Setup

        if (videoPlayer != null)
        {
            // Subscribe to video end event
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        #endregion
    }

    private void Update()
    {
        #region Timer Check

        if (loadAfterSeconds > 0f && !isLoading)
        {
            timer += Time.deltaTime;

            if (timer >= loadAfterSeconds)
            {
                LoadNextScene();
            }
        }

        #endregion

        #region Skip Input

        if (Input.GetKeyDown(skipKey) && !isLoading)
        {
            LoadNextScene();
        }

        #endregion
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        #region Video Finished

        if (!isLoading)
        {
            LoadNextScene();
        }

        #endregion
    }

    private void LoadNextScene()
    {
        #region Load Scene

        isLoading = true;

        // Optional: Stop video before switching
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        SceneManager.LoadScene(nextSceneName);

        #endregion
    }
}