using System;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody2D _rb; 
    public float bulletSpeed = 0;
    [SerializeField] private ParticleSystem explosionParticleEffect;

    private void OnEnable()
    {
        InputScript.OnFreezeBullet += HandleBulletFreezing;
    }
    
    private void OnDisable()
    {
        InputScript.OnFreezeBullet -= HandleBulletFreezing;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            throw new MissingComponentException("Rigidbody2D of Bullet is not found");
        }
    }
    
    public void Initialize(float newSpeed)
    {
        this.bulletSpeed = newSpeed;
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.right * bulletSpeed;
    }

    private void HandleBulletFreezing(bool isContinued)
    {
        _rb.constraints = isContinued ? RigidbodyConstraints2D.None : RigidbodyConstraints2D.FreezePosition;
        if (isContinued)
            explosionParticleEffect.Play();
        else
            explosionParticleEffect.Pause();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (explosionParticleEffect != null)
        {
            explosionParticleEffect.transform.SetParent(null);

            explosionParticleEffect.Play();

            float totalDuration = explosionParticleEffect.main.duration + explosionParticleEffect.main.startLifetime.constantMax;
            Destroy(explosionParticleEffect.gameObject, totalDuration);
            Destroy(gameObject);
        }
    }
}
