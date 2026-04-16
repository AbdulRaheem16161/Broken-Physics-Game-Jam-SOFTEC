using UnityEngine;

public class SimpleThirdPersonCamera : MonoBehaviour
{
    #region Target
    [Header("Target")]
    [SerializeField] private Transform target;
    #endregion

    #region Distance Settings
    [Header("Distance Settings")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float heightOffset = 2f;
    #endregion

    #region Rotation Settings
    [Header("Rotation Settings")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float minYAngle = -30f;
    [SerializeField] private float maxYAngle = 70f;
    #endregion

    #region Smooth Settings
    [Header("Smooth Settings")]
    [SerializeField] private float positionSmooth = 10f;
    #endregion

    #region Runtime Values
    private float currentX;
    private float currentY = 20f;
    #endregion

    private void Start()
    {
        #region Cursor Lock

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        #endregion
    }

    private void Update()
    {
        #region Input

        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        currentX += mouseX * mouseSensitivity;
        currentY -= mouseY * mouseSensitivity;

        currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);

        #endregion
    }

    private void LateUpdate()
    {
        #region Camera Movement

        if (target == null)
            return;

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);

        Vector3 offset = new Vector3(0f, heightOffset, -distance);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            positionSmooth * Time.deltaTime
        );

        transform.LookAt(target.position + Vector3.up * heightOffset);

        #endregion
    }
}