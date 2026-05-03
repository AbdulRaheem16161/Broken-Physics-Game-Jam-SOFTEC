using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/IncreaseSpeedPowerUp")]
public class IncreaseSpeedPowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerInstance.instance.playerHealth.Heal(healAmount);
    }
}
