using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    #region Health
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private float hp = 100f;
    [SerializeField] private bool cantDie = false;
    #endregion

    #region UI
    [SerializeField] private Image healthBar;
    #endregion

    #region Feedback
    [SerializeField] private DamageFeedback damageFeedback;
    [SerializeField] private DamageNumbers damageNumbers;
    #endregion

    private void Start()
    {
        if (hp <= 0f)
            hp = maxHp;

        if (damageFeedback == null)
            damageFeedback = GetComponent<DamageFeedback>();

        if (damageNumbers == null)
            damageNumbers = GetComponent<DamageNumbers>();

        UpdateHealthUI();
    }

    public void TakeDamage(float damage, GameObject attacker = null)
    {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0f, maxHp);

        if (damageFeedback != null)
            damageFeedback.PlayDamageFeedback();

        if (damageNumbers != null)
            damageNumbers.ShowDamage(damage);

        UpdateHealthUI();

        if (hp <= 0f && !cantDie)
            Destroy(gameObject);
    }

    private void UpdateHealthUI()
    {
        if (healthBar == null) return;
        healthBar.fillAmount = hp / maxHp;
    }
}