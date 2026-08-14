using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/HealPowerUp")]
public class HealPowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerManager.instance.playerHealth.Heal(healAmount);
    }
}
