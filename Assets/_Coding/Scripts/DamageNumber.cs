using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    #region Settings
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lifetime = 1f;

    [Header("Random Offset")]
    [SerializeField] private Vector3 randomOffsetRange = new Vector3(0.5f, 0.3f, 0f);
    #endregion

    #region References
    [SerializeField] private TMP_Text text;
    #endregion

    #region Runtime
    private float timer;
    private Color startColor;
    #endregion

    #region Unity Methods

    private void Awake()
    {
        #region Auto-Find Text (IMPORTANT FIX)

        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>(true);
        }

        if (text == null)
        {
            Debug.LogError("DamageNumber: No TMP_Text found in prefab!");
            return;
        }

        #endregion

        #region Setup Color

        startColor = text.color;

        #endregion

        #region Random Offset

        Vector3 offset = new Vector3(
            Random.Range(-randomOffsetRange.x, randomOffsetRange.x),
            Random.Range(-randomOffsetRange.y, randomOffsetRange.y),
            Random.Range(-randomOffsetRange.z, randomOffsetRange.z)
        );

        transform.position += offset;

        #endregion
    }

    private void Update()
    {
        #region Float Up

        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        #endregion

        #region Fade

        timer += Time.deltaTime;

        float t = timer / lifetime;

        Color c = startColor;
        c.a = Mathf.Lerp(1f, 0f, t);

        text.color = c;

        #endregion

        #region Destroy

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }

        #endregion
    }

    #endregion

    #region Public Method

    public void SetDamage(float damage)
    {
        #region FORCE TEXT UPDATE (NO FAIL POSSIBLE)

        if (text == null)
        {
            text = GetComponentInChildren<TMP_Text>(true);
        }

        if (text == null)
        {
            Debug.LogError("DamageNumber: TMP_Text missing at SetDamage!");
            return;
        }

        text.SetText("{0}", Mathf.RoundToInt(damage));

        #endregion
    }

    #endregion
}