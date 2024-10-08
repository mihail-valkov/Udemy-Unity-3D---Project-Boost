using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] float fuel = 100;
    [SerializeField] GameObject deathVFX;
    [SerializeField] ParticleSystem hitEffectPrefab;
    [SerializeField] ParticleSystem explodeEffectPrefab;
    [SerializeField] AudioClip[] explosionSounds;
    [SerializeField] AudioClip[] hitClips;
    [Range(0, 1)][SerializeField] float hitSoundVolume = 0.5f;
    [SerializeField] Material destroyedRocketMaterial; 
    [Range(0, 1)][SerializeField] float explosionSoundVolume = 0.5f;

    public int HealthValue
    {
        get { return health; }
    }

    public float FuelValue 
    { 
        get { return fuel; } 
    }

    public bool IsHealthTrackingActive { get; set; }

    void Awake()
    {
    }

    void Start()
    {
        //cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    private void KeepScore()
    {
        //GameManager.Instance.ScoreKeeper.AddScore(enemy.KillScore);
    }

    private void PlayHitEffect(Vector3 point)
    {
        if (hitEffectPrefab)
        {
            ParticleSystem hitEffect = Instantiate(hitEffectPrefab, new Vector3(point.x, point.y, -1f), Quaternion.identity);
            Destroy(hitEffect.gameObject, hitEffect.main.duration + hitEffect.main.startLifetime.constantMax);
        }
        
        GameManager.Instance.PlayRandomClip(hitClips, hitSoundVolume, transform.position);
    }

    private void TriggerDeathFX()
    {
        ParticleSystem hitEffect = Instantiate(explodeEffectPrefab, transform.position, Quaternion.identity);
        Destroy(hitEffect.gameObject, hitEffect.main.duration + hitEffect.main.startLifetime.constantMax);
        GameManager.Instance.PlayRandomClip(explosionSounds, explosionSoundVolume, new Vector3(transform.position.x, transform.position.y, -1f));

        //find all mesh renderes and set material destroyed rocket
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.material = destroyedRocketMaterial;
        }
    }

    public void TakeDamage(float value, Vector3 point)
    {
        if (!IsHealthTrackingActive)
        {
            return;
        }

        health -= (int)value;
        if (health <= 0)
        {
            IsHealthTrackingActive = false;
            TriggerDeathFX();
            GameManager.Instance.LevelFailed();
        }
        else
        {
            PlayHitEffect(point);
            //cameraShake.Shake();
        }
    }

    public void TakeFuel(float value)
    {
        this.fuel -= (float)value;
        if (this.fuel <= 0)
        {
            GameManager.Instance.LevelFailed();
        }
    }
}
