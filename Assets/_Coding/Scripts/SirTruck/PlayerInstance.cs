using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Health playerHealth;
    private ShotGun shotGun;
    private MachineGun machineGun;

    public static PlayerManager instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);

            return;
        }

        instance = this;

        if (playerHealth == null)
        {
            playerHealth = GetComponent<Health>();
        }
    }

    public void EnableShotGun()
    {
        if (shotGun != null)
        {
            shotGun.enabled = true;
        }
    }

    public void EnableMachineGun()
    {
        if (machineGun != null)
        {
            machineGun.enabled = true;
        }
    }
}