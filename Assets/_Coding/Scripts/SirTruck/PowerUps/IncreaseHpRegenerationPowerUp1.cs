using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/IncreaseHpRegenerationPowerUp")]
public class IncreaseHpRegenerationPowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerManager.instance.playerHealth.Heal(healAmount);
    }
}
