using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public Health playerHealth;

    public static PlayerInstance instance;
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
}