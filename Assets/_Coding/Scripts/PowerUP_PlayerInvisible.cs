using UnityEngine;

public class PowerUP_PlayerInvisible : MonoBehaviour, IPowerUp
{
    #region Settings

    [SerializeField] private string displayName = "No Collisions";

    public string DisplayName => displayName;

    [Header("Layers")]
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private string enemyLayerName = "Enemy";

    #endregion

    #region Runtime

    private int playerLayer;
    private int enemyLayer;

    #endregion

    private void Awake()
    {
        #region Cache Layers

        playerLayer = LayerMask.NameToLayer(playerLayerName);
        enemyLayer = LayerMask.NameToLayer(enemyLayerName);

        Debug.Log("[PowerUP_PlayerInvisible] Player Layer: " + playerLayer);
        Debug.Log("[PowerUP_PlayerInvisible] Enemy Layer: " + enemyLayer);

        #endregion
    }

    #region IPowerUp

    public void ActivatePowerUp()
    {
        #region Activate Invisibility (Disable Collision)

        if (playerLayer == -1 || enemyLayer == -1)
        {
            Debug.LogWarning("[PowerUP_PlayerInvisible] Layer not found!");
            return;
        }

        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        Debug.Log("[PowerUP_PlayerInvisible] ACTIVATED → Player is invisible to enemies");

        #endregion
    }

    public void DeactivatePowerUp()
    {
        #region Deactivate Invisibility (Enable Collision)

        if (playerLayer == -1 || enemyLayer == -1)
        {
            Debug.LogWarning("[PowerUP_PlayerInvisible] Layer not found!");
            return;
        }

        Physics.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        Debug.Log("[PowerUP_PlayerInvisible] DEACTIVATED → Player is visible again");

        #endregion
    }

    #endregion
}