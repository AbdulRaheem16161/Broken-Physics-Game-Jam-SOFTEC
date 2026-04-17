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

    #region Audio

    [Header("Hit Sounds")]
    [SerializeField] private List<AudioClip> hitClips = new List<AudioClip>();

    [Header("Spawn Sounds")]
    [SerializeField] private List<AudioClip> spawnClips = new List<AudioClip>();

    [SerializeField] private float volume = 1f;
    [SerializeField] private Vector2 pitchVariation = new Vector2(0.95f, 1.05f);

    private AudioSource audioSource;

    #endregion

    #region State

    private enum ProjectileState { Normal, PassedSacred }
    private ProjectileState _state = ProjectileState.Normal;
    private bool _destroyed = false;

    #endregion

    private void Awake()
    {
        #region Audio Setup

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 60f;
        audioSource.playOnAwake = false;

        PlaySpawnSound();

        #endregion
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_destroyed) return;

        int layerBit = 1 << other.gameObject.layer;

        bool isTarget = (targetLayer.value & layerBit) != 0;
        bool isSacred = (sacredLayer.value & layerBit) != 0;

        if (isSacred)
        {
            _state = ProjectileState.PassedSacred;
            return;
        }

        if (isTarget)
        {
            Health health = other.GetComponent<Health>();

            if (health != null)
            {
                float finalDamage = GetRandomDamage();
                health.TakeDamage(finalDamage, attacker);
            }

            PlayHitSound();
            DestroyProjectile();
            return;
        }

        DestroyProjectile();
    }

    private float GetRandomDamage()
    {
        float offset = Random.Range(minDamageOffset, maxDamageOffset);
        return Mathf.Max(0f, damage + offset);
    }

    private void PlaySpawnSound()
    {
        if (spawnClips.Count == 0) return;

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(spawnClips[Random.Range(0, spawnClips.Count)], volume);
    }

    private void PlayHitSound()
    {
        if (hitClips.Count == 0) return;

        audioSource.pitch = Random.Range(pitchVariation.x, pitchVariation.y);
        audioSource.PlayOneShot(hitClips[Random.Range(0, hitClips.Count)], volume);
    }

    private void DestroyProjectile()
    {
        _destroyed = true;
        Destroy(gameObject);
    }
}