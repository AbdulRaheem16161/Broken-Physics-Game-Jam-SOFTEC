using UnityEngine;
using System.Collections;

public class DamageFeedback : MonoBehaviour
{
    #region References
    [SerializeField] private Transform targetBody;
    #endregion

    #region Animation Settings
    [SerializeField] private float scaleMultiplier = 1.15f;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private AnimationCurve punchCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool restartOnDamage = true;
    #endregion

    #region Cooldown Settings
    [SerializeField] private float cooldown = 0.1f;
    #endregion

    #region Private State
    private Vector3 originalScale;
    private Coroutine feedbackCoroutine;
    private float lastPlayTime = -Mathf.Infinity;
    #endregion

    #region Initialization
    private void OnEnable()
    {
        if (targetBody != null)
        {
            originalScale = targetBody.localScale;
        }
    }

    private void OnValidate()
    {
        if (targetBody == null)
        {
            targetBody = GetComponentInChildren<Transform>();
        }

        scaleMultiplier = Mathf.Clamp(scaleMultiplier, 1.01f, 2f);
        duration = Mathf.Max(duration, 0.05f);
        cooldown = Mathf.Max(0f, cooldown);

        if (punchCurve.keys.Length == 0)
        {
            punchCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
    }
    #endregion

    #region Public API
    public void PlayDamageFeedback()
    {
        if (targetBody == null)
        {
            Debug.LogWarning("DamageFeedback: Target body is not assigned!", gameObject);
            return;
        }

        // 🧊 COOLDOWN CHECK (this is the hero of today)
        if (Time.time < lastPlayTime + cooldown)
        {
            return;
        }

        lastPlayTime = Time.time;

        if (restartOnDamage && feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
            targetBody.localScale = originalScale;
        }

        if (feedbackCoroutine == null || restartOnDamage)
        {
            feedbackCoroutine = StartCoroutine(PlayPunchFeedback());
        }
    }
    #endregion

    #region Animation Logic
    private IEnumerator PlayPunchFeedback()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float curveValue = punchCurve.Evaluate(t);

            float scaleT = t < 0.5f ? t * 2f : (1f - t) * 2f;
            float currentScale = Mathf.Lerp(1f, scaleMultiplier, curveValue * scaleT);

            targetBody.localScale = originalScale * currentScale;

            yield return null;
        }

        targetBody.localScale = originalScale;
        feedbackCoroutine = null;
    }
    #endregion
}