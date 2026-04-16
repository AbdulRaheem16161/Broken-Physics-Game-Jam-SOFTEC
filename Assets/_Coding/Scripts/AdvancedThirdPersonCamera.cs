using UnityEngine;

public class SimpleThirdPersonCamera : MonoBehaviour
{
    #region Target
    [Header("Target")]
    [SerializeField] private Transform target;
    #endregion

    #region Distance Settings
    [Header("Base Distance Settings")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float heightOffset = 2f;
    #endregion

    #region Speed Based Camera (NEW)
    [Header("Speed Based Camera Zoom Out")]
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private float speedToDistanceMultiplier = 0.02f;
    [SerializeField] private float maxExtraDistance = 6f;
    [SerializeField] private float distanceSmoothness = 5f;
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

    private float currentExtraDistance;
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
        #region Safety Check

        if (target == null)
            return;

        #endregion

        #region SPEED BASED CAMERA LOGIC (NEW)

        float speed = 0f;

        if (targetRigidbody != null)
        {
            speed = targetRigidbody.linearVelocity.magnitude;
        }

        float targetExtraDistance =
            Mathf.Clamp(speed * speedToDistanceMultiplier, 0f, maxExtraDistance);

        currentExtraDistance = Mathf.Lerp(
            currentExtraDistance,
            targetExtraDistance,
            distanceSmoothness * Time.deltaTime
        );

        #endregion

        #region Camera Movement

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);

        float finalDistance = distance + currentExtraDistance;

        Vector3 offset = new Vector3(0f, heightOffset, -finalDistance);
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