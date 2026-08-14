using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/IncreaseDamagePowerUp")]
public class IncreaseDamagePowerUp : BasePowerUp
{
    [SerializeField] private float healAmount = 20f;
    protected override void functionality()
    {
        PlayerManager.instance.playerHealth.Heal(healAmount);
    }
}
