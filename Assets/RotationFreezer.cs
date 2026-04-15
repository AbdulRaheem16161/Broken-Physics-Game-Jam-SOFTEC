using UnityEngine;

public class RotationFreezer : MonoBehaviour
{
    #region Unity Methods

    private void Update()
    {
        #region Freeze Local Rotation

        // Force rotation to zero relative to parent every frame
        transform.localRotation = Quaternion.identity;

        #endregion
    }

    #endregion
}