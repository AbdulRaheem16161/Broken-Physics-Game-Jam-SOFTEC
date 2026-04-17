using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    #region Settings

    [Header("Movement Detection")]
    [SerializeField] private float movementThreshold = 0.05f;

    [Header("Idle Time Before Disable")]
    [SerializeField] private float idleTimeRequired = 1f;

    #endregion

    #region Runtime Values

    private Vector3 lastPosition;
    private float currentVelocity;
    private float idleTimer;
    private bool hasDisabled;

    #endregion

    #region Unity Methods

    private void Start()
    {
        #region Initialization

        lastPosition = transform.position;

        #endregion
    }

    private void Update()
    {
        #region Update Logic

        if (hasDisabled) return;

        CalculateVelocity();
        HandleIdleCheck();

        lastPosition = transform.position;

        #endregion
    }

    #endregion

    #region Movement Logic

    private void CalculateVelocity()
    {
        #region Velocity Calculation

        float distance = Vector3.Distance(transform.position, lastPosition);
        currentVelocity = distance / Time.deltaTime;

        #endregion
    }

    private void HandleIdleCheck()
    {
        #region Idle Detection

        if (currentVelocity < movementThreshold)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTimeRequired)
            {
                DisableSelf();
            }
        }
        else
        {
            idleTimer = 0f;
        }

        #endregion
    }

    #endregion

    #region Disable Logic

    private void DisableSelf()
    {
        #region Disable Object

        hasDisabled = true;
        gameObject.SetActive(false);

        #endregion
    }

    #endregion
}