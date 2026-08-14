using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/WeaponMachineGunPowerUp")]
public class WeaponMachineGunPowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerManager.instance.EnableMachineGun();
    }
}