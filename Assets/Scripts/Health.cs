using UnityEngine;

public class Health : MonoBehaviour
{
    public float hp = 100f;

    public bool cantDie;

    public void TakeDamage(float damage, GameObject attacker)
    {
        hp -= damage;

        if (hp <= 0)
        {
            if (cantDie) return;
            Destroy(gameObject);
        }
    }
}