using UnityEngine;

public class EnemiesList : MonoBehaviour
{
    public static EnemiesList instance;

    void Awake()
    {
        if (instance != null)
        {
            destroy(this.gameobject);
        }

        instace = this;
    }
    
}
