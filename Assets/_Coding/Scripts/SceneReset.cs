using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScene : MonoBehaviour
{
    [Header("Reset Settings")]
    [SerializeField] private KeyCode resetKey = KeyCode.R;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private bool disableReset = false;

    private float inputTimer = 0f;
    private float deathTimer = 0f;
    private bool deathTriggered = false;

    private Health playerHealth;

    private void Start()
    {
        playerHealth = FindObjectOfType<Health>();

        if (playerHealth != null)
        {
            playerHealth.OnDeath += OnPlayerDied;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnDeath -= OnPlayerDied;
        }
    }

    private void Update()
    {
        HandleManualReset();
        HandleDeathReset();
    }

    private void HandleManualReset()
    {
        if (disableReset) return;

        if (Input.GetKey(resetKey))
        {
            inputTimer += Time.deltaTime;

            if (inputTimer >= holdDuration)
                ReloadScene();
        }
        else
        {
            inputTimer = 0f;
        }
    }

    private void HandleDeathReset()
    {
        if (!deathTriggered) return;

        deathTimer += Time.deltaTime;

        if (deathTimer >= holdDuration)
            ReloadScene();
    }

    private void OnPlayerDied()
    {
        deathTriggered = true;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}