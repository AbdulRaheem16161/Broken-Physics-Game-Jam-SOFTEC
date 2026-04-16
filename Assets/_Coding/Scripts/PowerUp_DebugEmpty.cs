using UnityEngine;

public class PowerUp_DebugEmpty : MonoBehaviour, IPowerUp
{
    #region IPowerUp

    public void ActivatePowerUp()
    {
        #region Activate

        Debug.Log("PowerUp Activated: " + gameObject.name);

        #endregion
    }

    public void DeactivatePowerUp()
    {
        #region Deactivate

        Debug.Log("PowerUp Deactivated: " + gameObject.name);

        #endregion
    }

    #endregion
}