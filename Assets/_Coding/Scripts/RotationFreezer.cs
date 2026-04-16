using UnityEngine;

public class RotationFreezer : MonoBehaviour
{
    #region Settings

    [Header("Rotation Settings")]

    // The rotation you want to lock to (in degrees)
    [SerializeField] private Vector3 targetRotationEuler = Vector3.zero;

    // Choose whether to use local or global rotation
    [SerializeField] private bool useLocalRotation = true;

    #endregion


    #region Unity Methods

    private void Update()
    {
        #region Freeze Rotation

        Quaternion targetRotation = Quaternion.Euler(targetRotationEuler);

        if (useLocalRotation)
        {
            // Lock local rotation
            transform.localRotation = targetRotation;
        }
        else
        {
            // Lock world rotation
            transform.rotation = targetRotation;
        }

        #endregion
    }

    #endregion
}