using UnityEngine;
using System;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    
    // Actions allow other scripts to "listen" for a pause without constant checking
    public static event Action<bool> OnPauseStateChanged;

    [Header("UI")]
    [SerializeField] private GameObject pauseUI;

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.P;

    private void Awake()
    {
        // Force unpause on fresh start
        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(pauseKey)) ////////////// PAUSE
        //{
        //    TogglePause();
        //}
    }

    public void TogglePause()
    {
        SetPauseState(!IsPaused);
    }

    public void SetPauseState(bool pause)
    {
        IsPaused = pause;
        
        // 1. Physical Time
        Time.timeScale = pause ? 0f : 1f;

        // 2. Audio
        AudioListener.pause = pause;

        // 3. UI and Cursor
        if (pauseUI != null) pauseUI.SetActive(pause);
        
        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;

        // 4. Notify other scripts
        OnPauseStateChanged?.Invoke(pause);
        
        Debug.Log(pause ? "== GAME PAUSED ==" : "== GAME RESUMED ==");
    }
}