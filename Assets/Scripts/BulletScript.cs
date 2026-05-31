using System;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody2D _rb;
    [SerializeField] private float bulletSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            throw new MissingComponentException("Rigidbody2D of Bullet is not found");
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = transform.right * bulletSpeed;
    }
}
