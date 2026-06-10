using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject ghostTurret;
    [SerializeField] private float interval = 1f;

    private float _distance;
    private float _angle;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private GameObject bulletPrefab;
    private float _timer = 0;
    public static bool KeepShooting;
    private RaycastHit2D _hit;
    
    private void Awake()
    {
        KeepShooting = true;
    }
    void Update()
    {
        if (!KeepShooting) return;
        CalculateTurretAngleAndDistanceFromPlayer();
        GhostFollowPlayer();
        _hit = Physics2D.Raycast(transform.position, ghostTurret.transform.right, _distance, LayerMask.GetMask("Tile"));
        
        if (_hit) return;
        
        TurretFollowPlayer();
        _timer += Time.deltaTime;
        if (_timer < interval) return;
        
        Instantiate(bulletPrefab, new Vector3(transform.position.x, transform.position.y, 11), transform.rotation);
        _timer = 0;
    }

    private void CalculateTurretAngleAndDistanceFromPlayer()
    {
        _distance = Vector2.Distance(playerTransform.position, transform.position);
        
        // angle will be used to change the angle the turret to face the player
        _angle = Mathf.Atan2(playerTransform.transform.position.y - transform.position.y, playerTransform.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
    }
    
    private void GhostFollowPlayer()
    {
        ghostTurret.transform.rotation = Quaternion.Euler(0, 0, _angle);
        Debug.DrawRay(transform.position, ghostTurret.transform.right * _distance, Color.red);
    }

    private void TurretFollowPlayer()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            Quaternion.Euler(0, 0, _angle), 
            rotationSpeed * Time.deltaTime);
    }
}
