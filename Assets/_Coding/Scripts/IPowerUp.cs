public interface IPowerUp
{
    void ActivatePowerUp();
    void DeactivatePowerUp();

    string DisplayName { get; }
}