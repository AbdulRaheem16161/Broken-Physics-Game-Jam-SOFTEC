using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    #region Damage Settings

    public float damage = 10f;

    [SerializeField] private float minDamageOffset = -5f;
    [SerializeField] private float maxDamageOffset = 5f;

    #endregion

    #region Lifetime

    [SerializeField] private float lifeTime = 5f;
    [HideInInspector] public GameObject attacker;

    #endregion

    #region Layers

    [Header("Layers")]
    public LayerMask targetLayer;
    public LayerMask sacredLayer;

    #endregion

    #region 🔊 Audio (NEW)

    [Header("Hit / Impact Sounds")]
    [SerializeField] private List<AudioClip> hitClips = new List<AudioClip>();

    [Header("Spawn / Launch Sound (PLAYS ONCE)")]
    [SerializeField] private List<AudioClip> spawnClips = new List<AudioClip>();

    [Header("Audio Settings")]
    [SerializeField] private float volume = 1f;
    [SerializeField] private Vector2 pitchVariation = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;

    #endregion

    #region State

    private enum ProjectileState
    {
        Normal,
        PassedSacred
    }

    private ProjectileState _state = ProjectileState.Normal;
    private bool _destroyed = false;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        #region Audio Setup

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // FULL 3D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 60f;
        audioSource.playOnAwake = false;

        PlaySpawnSound(); // 🔥 PLAY ONCE HERE

        #endregion
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (_destroyed) return;

        int otherLayerBit = 1 << other.gameObject.layer;
        bool isTarget = (targetLayer.value & otherLayerBit) != 0;
        bool isSacred = (sacredLayer.value & otherLayerBit) != 0;

        #region Sacred Handling

        if (isSacred)
        {
            _state = ProjectileState.PassedSacred;
            return;
        }

        #endregion

        #region Target Handling

        if (isTarget)
        {
            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                float finalDamage = GetRandomDamage();
                health.TakeDamage(finalDamage, attacker);
            }

            PlayHitSound(); // 🔊 NEW

            DestroyProjectile();
            return;
        }

        #endregion

        #region Default Collision

        DestroyProjectile();

        #endregion
    }

    #endregion

    #region Damage

    private float GetRandomDamage()
    {
        float randomOffset = Random.Range(minDamageOffset, maxDamageOffset);
        float finalDamage = damage + randomOffset;

        return Mathf.Max(0f, finalDamage);
    }

    #endregion

    #region 🔊 Audio Logic (NEW)

    private void PlaySpawnSound()
    {
        if (spawnClips == null || spawnClips.Count == 0 || audioSource == null)
            return;

        AudioClip clip = spawnClips[Random.Range(0, spawnClips.Count)];

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(clip, volume);
    }

    private void PlayHitSound()
    {
        if (hitClips == null || hitClips.Count == 0 || audioSource == null)
            return;

        AudioClip clip = hitClips[Random.Range(0, hitClips.Count)];

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(clip, volume);
    }

    #endregion

    #region Destroy

    private void DestroyProjectile()
    {
        _destroyed = true;
        Destroy(gameObject);
    }

    #endregion
}