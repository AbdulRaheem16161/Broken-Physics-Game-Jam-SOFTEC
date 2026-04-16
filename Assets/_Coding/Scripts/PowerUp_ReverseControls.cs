using UnityEngine;
using ArcadeVP;

public class PowerUp_ReverseControls : MonoBehaviour, IPowerUp
{
    #region References

    [Header("Input System")]
    [SerializeField] private InputManager_ArcadeVP inputManager;

    #endregion

    private void Awake()
    {
        #region Auto Assign

        if (inputManager == null)
        {
            inputManager = GetComponent<InputManager_ArcadeVP>();
        }

        Debug.Log("[ReverseControls] Ready -> " + (inputManager != null));

        #endregion
    }

    #region IPowerUp

    public void ActivatePowerUp()
    {
        #region Enable Reverse Controls

        if (inputManager == null) return;

        inputManager.reverseControls = true;

        Debug.Log("[ReverseControls] ACTIVATED");

        #endregion
    }

    public void DeactivatePowerUp()
    {
        #region Disable Reverse Controls

        if (inputManager == null) return;

        inputManager.reverseControls = false;

        Debug.Log("[ReverseControls] DEACTIVATED");

        #endregion
    }

    #endregion
}