using UnityEngine;

public class MusicManager : MonoBehaviour
{
    #region Settings

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip backgroundMusic;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    #endregion

    #region Runtime

    private bool isMuted = false;

    #endregion

    #region Unity Events

    private void Start()
    {
        #region Start Music
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
        #endregion
    }

    #endregion

    #region Volume Controls

    public void IncreaseVolume(float amount)
    {
        #region Increase Volume
        musicVolume = Mathf.Clamp01(musicVolume + amount);
        ApplyVolume();
        #endregion
    }

    public void DecreaseVolume(float amount)
    {
        #region Decrease Volume
        musicVolume = Mathf.Clamp01(musicVolume - amount);
        ApplyVolume();
        #endregion
    }

    public void SetVolume(float value)
    {
        #region Set Volume Directly
        musicVolume = Mathf.Clamp01(value);
        ApplyVolume();
        #endregion
    }

    private void ApplyVolume()
    {
        #region Apply Volume To Source
        if (musicSource != null)
        {
            musicSource.volume = isMuted ? 0f : musicVolume;
        }
        #endregion
    }

    #endregion

    #region Mute Controls

    public void ToggleMute()
    {
        #region Toggle Mute
        isMuted = !isMuted;
        ApplyVolume();
        #endregion
    }

    #endregion
}