using UnityEngine;

public class SizeMultiplierHelper : MonoBehaviour
{
    #region Enums

    public enum CarType
    {
        PhysicsCar,
        NavAgentCar,
        BouncyCar
    }

    #endregion

    #region Settings

    public CarType carType;

    [Header("Toggle")]
    public bool increaseSize = false;

    [Header("Scale")]
    [SerializeField] private Vector3 targetScale = new Vector3(3f, 3f, 3f);

    [Header("Health Boost")]
    [SerializeField] private float healthIncrease = 100f;

    [Header("Damage Feedback")]
    [SerializeField] private float newScaleMultiplier = 1.15f;

    #endregion

    #region Cached References

    private Transform meshTransform;
    private MaceImpact maceImpact;
    private Health health;
    private DamageFeedback damageFeedback;

    #endregion

    #region Stored Defaults

    private Vector3 originalScale;
    private Vector3 originalBoxSize;
    private Vector3 originalOffset;
    private float originalMaxHp;
    private float originalHp;
    private float originalFeedbackScale;

    private bool hasStoredDefaults = false;

    #endregion

    private void Awake()
    {
        #region Find References

        meshTransform = FindMeshChild(transform);
        maceImpact = GetComponentInChildren<MaceImpact>();
        health = GetComponent<Health>();
        damageFeedback = GetComponent<DamageFeedback>();

        #endregion
    }

    private void Update()
    {
        #region Toggle Logic

        if (increaseSize)
        {
            ApplyIncrease();
        }
        else
        {
            ResetValues();
        }

        #endregion
    }

    #region Core Logic

    private void ApplyIncrease()
    {
        #region Store Defaults

        if (!hasStoredDefaults)
        {
            if (meshTransform != null)
                originalScale = meshTransform.localScale;

            if (maceImpact != null)
            {
                originalBoxSize = GetBoxSize(maceImpact);
                originalOffset = GetOffset(maceImpact);
            }

            if (health != null)
            {
                originalMaxHp = GetMaxHp(health);
                originalHp = GetHp(health);
            }

            if (damageFeedback != null)
            {
                originalFeedbackScale = GetFeedbackScale(damageFeedback);
            }

            hasStoredDefaults = true;
        }

        #endregion

        #region Apply Per Car Type

        switch (carType)
        {
            case CarType.PhysicsCar:
                ApplyPhysicsCar();
                break;

            case CarType.NavAgentCar:
                ApplyNavAgentCar();
                break;

            case CarType.BouncyCar:
                ApplyBouncyCar();
                break;
        }

        #endregion
    }

    private void ResetValues()
    {
        #region Reset Only If Stored

        if (!hasStoredDefaults) return;

        if (meshTransform != null)
            meshTransform.localScale = originalScale;

        if (maceImpact != null)
        {
            SetBoxSize(maceImpact, originalBoxSize);
            SetOffset(maceImpact, originalOffset);
            maceImpact.transform.localScale = Vector3.one;
        }

        if (health != null)
        {
            SetMaxHp(health, originalMaxHp);
            SetHp(health, originalHp);
        }

        if (damageFeedback != null)
        {
            SetFeedbackScale(damageFeedback, originalFeedbackScale);
        }

        hasStoredDefaults = false;

        #endregion
    }

    #endregion

    #region Car Type Implementations

    private void ApplyPhysicsCar()
    {
        if (meshTransform != null)
            meshTransform.localScale = targetScale;

        if (maceImpact != null)
        {
            SetBoxSize(maceImpact, new Vector3(5.1f, 1.32f, 12.09f));
            SetOffset(maceImpact, new Vector3(0f, 0.43f, 0f));
        }

        ApplySharedEffects();
    }

    private void ApplyNavAgentCar()
    {
        if (meshTransform != null)
            meshTransform.localScale = targetScale;

        if (maceImpact != null)
        {
            SetBoxSize(maceImpact, new Vector3(4.95f, 1.97f, 13.19f));
            SetOffset(maceImpact, new Vector3(0f, -1.78f, 0f));
        }

        ApplySharedEffects();
    }

    private void ApplyBouncyCar()
    {
        if (meshTransform != null)
            meshTransform.localScale = targetScale;

        if (maceImpact != null)
            maceImpact.transform.localScale = targetScale;

        ApplySharedEffects();
    }

    private void ApplySharedEffects()
    {
        if (health != null)
        {
            SetMaxHp(health, originalMaxHp + healthIncrease);
            SetHp(health, originalHp + healthIncrease);
        }

        if (damageFeedback != null)
        {
            SetFeedbackScale(damageFeedback, newScaleMultiplier);
        }
    }

    #endregion

    #region Helpers

    private Transform FindMeshChild(Transform parent)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("Mesh"))
                return child;
        }
        return null;
    }

    private Vector3 GetBoxSize(MaceImpact m) => (Vector3)typeof(MaceImpact).GetField("boxSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(m);
    private void SetBoxSize(MaceImpact m, Vector3 val) => typeof(MaceImpact).GetField("boxSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(m, val);

    private Vector3 GetOffset(MaceImpact m) => (Vector3)typeof(MaceImpact).GetField("offset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(m);
    private void SetOffset(MaceImpact m, Vector3 val) => typeof(MaceImpact).GetField("offset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(m, val);

    private float GetMaxHp(Health h) => (float)typeof(Health).GetField("maxHp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(h);
    private void SetMaxHp(Health h, float val) => typeof(Health).GetField("maxHp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(h, val);

    private float GetHp(Health h) => (float)typeof(Health).GetField("hp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(h);
    private void SetHp(Health h, float val) => typeof(Health).GetField("hp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(h, val);

    private float GetFeedbackScale(DamageFeedback d) => (float)typeof(DamageFeedback).GetField("scaleMultiplier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(d);
    private void SetFeedbackScale(DamageFeedback d, float val) => typeof(DamageFeedback).GetField("scaleMultiplier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(d, val);

    #endregion
}