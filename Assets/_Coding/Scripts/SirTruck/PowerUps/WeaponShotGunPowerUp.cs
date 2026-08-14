using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/WeaponShotGunPowerUp")]
public class WeaponShotGunPowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerManager.instance.EnableShotGun();
    }
}
