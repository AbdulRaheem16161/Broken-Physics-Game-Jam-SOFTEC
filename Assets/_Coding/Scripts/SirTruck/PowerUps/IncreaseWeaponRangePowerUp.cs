using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/IncreaseWeaponRangePowerUp")]
public class IncreaseWeaponRangePowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerManager.instance.playerHealth.Heal(healAmount);
    }
}
