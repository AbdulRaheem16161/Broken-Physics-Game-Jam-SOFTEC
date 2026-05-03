using UnityEngine;
public abstract class BasePowerUp : ScriptableObject
{
    [SerializeField] private string powerUpName;
    public Sprite powerUpImage;

    public void ActivatePowerUp()
    {
        functionality();
    }
    protected abstract void functionality();
}
