using System.Collections;
using UnityEngine;

public class TurretScript : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject ghostTurret;
    [SerializeField] private ParticleSystem fireParticleEffect;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float interval = 1f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool trackPlayer = false;
    [SerializeField] private bool followPlayer = false;
    [SerializeField] private bool delayFiring = false;
    [SerializeField] private float delayAmount = 0.0f;

    private float _distance;
    private float _angle;
    private float _timer = 0;
    private RaycastHit2D _hit;
    private bool keepShooting = true;

    private void OnEnable()
    {
        PlayerScript.TurretFireControl += HandleTurretFireControls;
        InputScript.TurretFireControl += HandleTurretFireControls;
    }

    private void OnDisable()
    {
        PlayerScript.TurretFireControl -= HandleTurretFireControls;
        InputScript.TurretFireControl += HandleTurretFireControls;
    }

    private void Start()
    {
        if (delayFiring)
        {
            keepShooting = false;
            StartCoroutine(DelayFiring());
        }
    }

    private IEnumerator DelayFiring()
    {
        yield return new WaitForSeconds(delayAmount);
        Debug.Log("delayed");
        keepShooting = true;
    }

    private void Update()
    {
        if (!keepShooting) return;

        if (trackPlayer)
        {
            CalculateTurretAngleAndDistanceFromPlayer();
            GhostFollowPlayer();
            _hit = Physics2D.Raycast(transform.position, ghostTurret.transform.right, _distance,
                LayerMask.GetMask("Tile"));
            if (!_hit)
            {
                AlignTurretRotationToFacePlayer();
            }
        }

        if (followPlayer)
        {
            // This might be a mechanic for the turret in the end level where it follows the player around the map
            return;
        }

        _timer += Time.deltaTime;
        if (_timer >= interval)
        {
            if (!_hit)
            {
                fireParticleEffect.Play();
                FireBulette();
            }
            _timer = 0;
        }
    }

    private void FireBulette()
    {
        GameObject bulletCopy = Instantiate(bullet, new Vector3(transform.position.x, transform.position.y, 11),
            transform.rotation);
        BulletScript bulletScript = bulletCopy.GetComponent<BulletScript>();
        if (bulletScript)
        {
            bulletScript.Initialize(bulletSpeed);
        }
    }

    private void CalculateTurretAngleAndDistanceFromPlayer()
    {
        _distance = Vector2.Distance(playerTransform.position, transform.position);

        // angle will be used to change the angle the turret to face the player
        _angle = Mathf.Atan2(playerTransform.transform.position.y - transform.position.y,
            playerTransform.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
    }

    private void GhostFollowPlayer()
    {
        ghostTurret.transform.rotation = Quaternion.Euler(0, 0, _angle);
        Debug.DrawRay(transform.position, ghostTurret.transform.right * _distance, Color.red);
    }

    private void AlignTurretRotationToFacePlayer()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.Euler(0, 0, _angle),
            rotationSpeed * Time.deltaTime);
    }

    private bool HandleTurretFireControls()
    {
        keepShooting = !keepShooting;
        return keepShooting;
    }
}
