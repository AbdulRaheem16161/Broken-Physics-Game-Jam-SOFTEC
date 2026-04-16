using UnityEngine;
using UnityEngine.UI;
using ArcadeVP;

public class Nitro : MonoBehaviour
{
    public ArcadeVehicleController vehicle;
    public Image nitroBar;

    public KeyCode nitroKey = KeyCode.Mouse0;

    [Header("Nitro Settings")]
    public float maxNitro = 100f;
    public float nitro;
    public float drainRate = 25f;
    public float regenRate = 15f;

    [Header("Boost Targets")]
    public float speedBoost = 1.6f;
    public float accelBoost = 1.4f;
    public float turnBoost = 1.2f;

    [Header("Smoothness (IMPORTANT)")]
    [Tooltip("High = instant response, Low = slow nitro build-up")]
    public float boostSmoothRate = 5f;

    private bool active;

    private void Start()
    {
        nitro = maxNitro;
    }

    private void Update()
    {
        HandleInput();
        HandleNitroFuel();
        SmoothBoostApplication();
        UpdateUI();
    }

    #region INPUT
    void HandleInput()
    {
        if (Input.GetKeyDown(nitroKey))
        {
            active = true;
        }

        if (Input.GetKeyUp(nitroKey))
        {
            active = false;
        }
    }
    #endregion

    #region FUEL SYSTEM
    void HandleNitroFuel()
    {
        if (active && nitro > 0)
        {
            nitro -= drainRate * Time.deltaTime;

            if (nitro <= 0)
            {
                nitro = 0;
                active = false;
            }
        }
        else
        {
            nitro += regenRate * Time.deltaTime;
        }

        nitro = Mathf.Clamp(nitro, 0, maxNitro);
    }
    #endregion

    #region SMOOTH BOOST SYSTEM (NEW CORE)
    void SmoothBoostApplication()
    {
        float targetSpeed = active && nitro > 0 ? speedBoost : 1f;
        float targetAccel = active && nitro > 0 ? accelBoost : 1f;
        float targetTurn = active && nitro > 0 ? turnBoost : 1f;

        float t = Time.deltaTime * boostSmoothRate;

        vehicle.targetSpeedMultiplier =
            Mathf.Lerp(vehicle.targetSpeedMultiplier, targetSpeed, t);

        vehicle.targetAccelerationMultiplier =
            Mathf.Lerp(vehicle.targetAccelerationMultiplier, targetAccel, t);

        vehicle.targetTurnMultiplier =
            Mathf.Lerp(vehicle.targetTurnMultiplier, targetTurn, t);
    }
    #endregion

    #region UI
    void UpdateUI()
    {
        if (nitroBar != null)
            nitroBar.fillAmount = nitro / maxNitro;
    }
    #endregion
}