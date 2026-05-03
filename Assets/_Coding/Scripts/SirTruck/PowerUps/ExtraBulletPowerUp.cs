using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/ExtraBulletPowerUp")]
public class ExtraBulletPowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerInstance.instance.playerHealth.Heal(healAmount);
    }
}
