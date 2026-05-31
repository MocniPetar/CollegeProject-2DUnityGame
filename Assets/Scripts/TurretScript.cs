using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    public static event Action FireTurretAnimation;
    [SerializeField] private float interval = 1f;

    private float _distance;
    private float _angle;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private GameObject bulletPrefab;
    private float _timer = 0;
    public static bool KeepShooting;
    private RaycastHit2D _hitWall;
    
    private void Awake()
    {
        KeepShooting = true;
    }

    void Update()
    {
        if (!KeepShooting) return;
        FollowPlayer();
        _hitWall = Physics2D.Raycast(transform.position, transform.right, _distance, LayerMask.GetMask("Tile"));
        
        if (_hitWall) return;
        
        _timer += Time.deltaTime;
        if (_timer < interval) return;
        
        Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, 11), transform.rotation);
        _timer = 0;
    }

    private void FollowPlayer()
    {
        // distance will be used in ray to detect if the player can be seen
        _distance = Vector2.Distance(playerTransform.position, transform.position);
        
        Debug.DrawRay(transform.position, transform.right * _distance, Color.red);
        
        // angle will be used to change the angle the turret to face the player
        _angle = Mathf.Atan2(playerTransform.transform.position.y - transform.position.y, playerTransform.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        
        Quaternion rotation = Quaternion.Euler(0, 0, _angle);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
