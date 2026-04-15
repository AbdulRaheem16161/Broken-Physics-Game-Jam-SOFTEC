using UnityEngine;

public class RKeyLiftAndReset : MonoBehaviour
{
    #region Unity Methods

    private void Update()
    {
        #region Input Check

        // Check if R key is pressed this frame
        if (Input.GetKeyDown(KeyCode.R))
        {
            ApplyLiftAndRotationReset();
        }

        #endregion
    }

    #endregion

    #region Action

    private void ApplyLiftAndRotationReset()
    {
        #region Position Update

        // Move object up by 2 units on Y axis
        transform.position += new Vector3(0f, 4f, 0f);

        #endregion

        #region Rotation Fix

        // Keep current rotation but force Z to 0
        Vector3 euler = transform.eulerAngles;
        euler.z = 0f;
        transform.eulerAngles = euler;

        #endregion
    }

    #endregion
}