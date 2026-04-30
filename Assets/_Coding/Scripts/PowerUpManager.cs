using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
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

    [System.Serializable]
    public class PowerUpEntry
    {
        public MonoBehaviour powerUpBehaviour;   // must implement IPowerUp
        public bool enabled = true;

        public StopMethod stopMethod = StopMethod.Timer;
        public float duration = 5f;

        [Header("Level Restriction")]
        public int maxLevel = int.MaxValue;

        [Header("Audio")]
        public AudioClip activationClip;
    }

    [Header("Mode")]
    [SerializeField] private Mode mode = Mode.Random;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI modeText;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Controlled Sequence")]
    [SerializeField] private List<PowerUpEntry> controlledSequence = new List<PowerUpEntry>();

    [Header("Random Pool")]
    [SerializeField] private List<PowerUpEntry> randomPool = new List<PowerUpEntry>();

    private int sequenceIndex = 0;
    private IPowerUp activePowerUp;
    private PowerUpEntry activeEntry;
    private Coroutine activeRoutine;

    public void ActivatePowerUpOnLevelUp(int currentLevel)
    {
        Debug.Log("PowerUp Manager Triggered Level Up PowerUp");

        if (activeEntry != null && activeEntry.stopMethod == StopMethod.UntilNextPowerUp)
        {
            activePowerUp?.DeactivatePowerUp();
            UpdateModeUI();
        }

        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            activeRoutine = null;
        }

        PowerUpEntry entry = GetNextPowerUp(currentLevel);

        if (entry == null || entry.powerUpBehaviour == null) return;

        activeEntry = entry;
        activePowerUp = entry.powerUpBehaviour as IPowerUp;

        if (activePowerUp == null)
        {
            Debug.LogWarning("PowerUp does not implement IPowerUp!");
            return;
        }

        activePowerUp.ActivatePowerUp();
        PlayPowerUpAudio(entry);
        UpdateModeUI();

        if (entry.stopMethod == StopMethod.Timer)
        {
            activeRoutine = StartCoroutine(StopAfterTime(entry.duration));
        }
    }

    private void PlayPowerUpAudio(PowerUpEntry entry)
    {
        if (audioSource == null) return;
        if (entry.activationClip == null) return;

        audioSource.PlayOneShot(entry.activationClip);
    }

    private PowerUpEntry GetNextPowerUp(int currentLevel)
    {
        if (mode == Mode.Random)
        {
            List<PowerUpEntry> valid = new List<PowerUpEntry>();

            foreach (var p in randomPool)
            {
                if (p == null) continue;

                if (!p.enabled) continue;
                if (p.powerUpBehaviour == null) continue;
                if (currentLevel > p.maxLevel) continue;

                valid.Add(p);
            }

            if (valid.Count == 0) return null;

            return valid[Random.Range(0, valid.Count)];
        }

        if (controlledSequence.Count == 0) return null;

        int attempts = 0;

        while (attempts < controlledSequence.Count)
        {
            PowerUpEntry entry = controlledSequence[sequenceIndex];
            sequenceIndex = (sequenceIndex + 1) % controlledSequence.Count;
            attempts++;

            if (entry == null) continue;
            if (!entry.enabled) continue;
            if (entry.powerUpBehaviour == null) continue;
            if (currentLevel > entry.maxLevel) continue;

            return entry;
        }

        return null;
    }

    private IEnumerator StopAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        activePowerUp?.DeactivatePowerUp();
        UpdateModeUI();
    }

    private void UpdateModeUI()
    {
        if (modeText == null) return;

        string displayName = "None";

        if (activePowerUp != null)
        {
            displayName = activePowerUp.DisplayName;
        }

        modeText.text = "Physics Mode: " + displayName;
    }
}