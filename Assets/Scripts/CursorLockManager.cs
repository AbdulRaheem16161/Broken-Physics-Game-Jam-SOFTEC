using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLockManager : MonoBehaviour
{
    #region Cursor State

    // Tracks whether cursor is currently locked
    [SerializeField] private bool isCursorLocked = false;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        #region Initialize Cursor State

        // Ensure cursor starts unlocked and visible
        UnlockCursor();

        #endregion
    }

    private void Update()
    {
        #region Input Handling

        // Lock cursor on left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }

        // Unlock cursor on Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        #endregion
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        #region Focus Handling

        // If game loses focus, unlock cursor for safety
        if (!hasFocus)
        {
            UnlockCursor();
        }

        #endregion
    }

    #endregion

    #region Cursor Control - Lock

    private void LockCursor()
    {
        #region Lock Cursor Logic

        // Set cursor state
        isCursorLocked = true;

        // Hide cursor from screen
        Cursor.visible = false;

        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;

        #endregion
    }

    #endregion

    #region Cursor Control - Unlock

    private void UnlockCursor()
    {
        #region Unlock Cursor Logic

        // Set cursor state
        isCursorLocked = false;

        // Show cursor again
        Cursor.visible = true;

        // Free cursor movement
        Cursor.lockState = CursorLockMode.None;

        #endregion
    }

    #endregion
}