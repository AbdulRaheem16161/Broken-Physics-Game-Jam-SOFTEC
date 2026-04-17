using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
    #region Enums

    public enum StopMethod
    {
        Timer,
        UntilNextPowerUp
    }

    public enum Mode
    {
        Random,
        Controlled
    }

    #endregion

    #region PowerUp Entry

    [System.Serializable]
    public class PowerUpEntry
    {
        public MonoBehaviour powerUpBehaviour;   // must implement IPowerUp
        public bool enabled = true;

        public StopMethod stopMethod = StopMethod.Timer;
        public float duration = 5f;
    }

    #endregion

    #region Settings

    [Header("Mode")]
    [SerializeField] private Mode mode = Mode.Random;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI modeText;

    [Header("Controlled Sequence")]
    [SerializeField] private List<PowerUpEntry> controlledSequence = new List<PowerUpEntry>();

    [Header("Random Pool")]
    [SerializeField] private List<PowerUpEntry> randomPool = new List<PowerUpEntry>();

    #endregion

    #region Runtime

    private int sequenceIndex = 0;
    private IPowerUp activePowerUp;
    private PowerUpEntry activeEntry;
    private Coroutine activeRoutine;

    #endregion

    #region Public Trigger

    public void ActivatePowerUpOnLevelUp()
    {
        Debug.Log("PowerUp Manager Triggered Level Up PowerUp");

        #region Deactivate Previous (if needed)

        if (activeEntry != null && activeEntry.stopMethod == StopMethod.UntilNextPowerUp)
        {
            activePowerUp?.DeactivatePowerUp();
            UpdateModeUI();
        }

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
        }

        #endregion

        #region Pick PowerUp

        PowerUpEntry entry = GetNextPowerUp();

        if (entry == null || entry.powerUpBehaviour == null) return;

        activeEntry = entry;
        activePowerUp = entry.powerUpBehaviour as IPowerUp;

        if (activePowerUp == null)
        {
            Debug.LogWarning("PowerUp does not implement IPowerUp!");
            return;
        }

        activePowerUp.ActivatePowerUp();

        UpdateModeUI();

        #endregion

        #region Handle Stop Methods

        if (entry.stopMethod == StopMethod.Timer)
        {
            activeRoutine = StartCoroutine(StopAfterTime(entry.duration));
        }

        #endregion
    }

    #endregion

    #region Power Selection

    private PowerUpEntry GetNextPowerUp()
    {
        #region Random Mode

        if (mode == Mode.Random)
        {
            List<PowerUpEntry> valid = new List<PowerUpEntry>();

            foreach (var p in randomPool)
            {
                if (p.enabled && p.powerUpBehaviour != null)
                    valid.Add(p);
            }

            if (valid.Count == 0) return null;

            return valid[Random.Range(0, valid.Count)];
        }

        #endregion

        #region Controlled Mode

        if (controlledSequence.Count == 0) return null;

        for (int i = 0; i < controlledSequence.Count; i++)
        {
            PowerUpEntry entry = controlledSequence[sequenceIndex];

            sequenceIndex = (sequenceIndex + 1) % controlledSequence.Count;

            if (entry.enabled && entry.powerUpBehaviour != null)
                return entry;
        }

        return null;

        #endregion
    }

    #endregion

    #region Coroutine

    private IEnumerator StopAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        activePowerUp?.DeactivatePowerUp();

        UpdateModeUI();
    }

    #endregion

    #region UI

    private void UpdateModeUI()
    {
        if (modeText == null) return;

        string displayName = "None";

        if (activePowerUp != null)
        {
            displayName = activePowerUp.DisplayName;
        }

        modeText.text = "Reality Mode: " + displayName;
    }

    #endregion
}