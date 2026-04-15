using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float maxHp = 100f;
    public float hp = 100f;

    public bool cantDie;

    // UI reference (assign your Image here)
    public Image healthBar;

    private void Start()
    {
        // Ensure UI starts correct
        UpdateHealthUI();
    }

    public void TakeDamage(float damage, GameObject attacker)
    {
        hp -= damage;

        // Clamp so it doesn't go below 0 (because negative health is just emotional damage)
        hp = Mathf.Clamp(hp, 0f, maxHp);

        UpdateHealthUI();

        if (hp <= 0)
        {
            if (cantDie) return;

            Destroy(gameObject);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar == null) return;

        // Convert health into 0–1 range for Image.fillAmount
        healthBar.fillAmount = hp / maxHp;
    }
}