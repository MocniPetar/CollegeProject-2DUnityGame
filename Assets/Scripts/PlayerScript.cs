using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerScript : MonoBehaviour
{
    // Player Animation Actions
    public static event Action<float> PlayerRunAnimation;
    public static event Action<bool> PlayerJumpAnimation;
    public static event Action<bool> PlayerWallGrabAnimation;
    public static event Action<bool> PlayerFallAnimation;
    public static event Action<bool> PlayerDashAnimation;
    public static event Action PlayerDeathAnimation;
    public static event Action<float> DashUIAnimation;
    public static event Func<bool> TurretFireControl;

    // Player components
    private Rigidbody2D _rigidBody2D;
    private SpriteRenderer _spriteRenderer;
    public Animator animator;
    
    // Other components
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject dashObject;
    [SerializeField] private ParticleSystem slideGrindArm;
    [SerializeField] private ParticleSystem slideGrindLeg;
    [SerializeField] private LayerMask tileLayer;
    
    // Player movement variables
    [SerializeField] private float gravityForce;
    
    // The constant speed a player moves when accelerated to that speed
    [SerializeField] private float constantSpeed;
    
    // The acceleration overtime to the const speed
    [SerializeField] private float accelerationFactor;
    // [SerializeField] private float accelerationSpeed;
    
    // The deacceleration overtime until the player completely stops
    [SerializeField] private float dampingForce;
    
    // Dashing
    //      - when the player dashes it starts a timer for the dash duration
    //      - after the dash duration finishes the timer for the cooldown is started
    //      - every timer is subtracted by Time.deltaTime
    private float _dashTime;
    private float _dashCooldownTime;
    private int _direction = 1;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashJump;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float  dashDistance;

    [SerializeField] private float jumpForce;
    private bool _canJumpFromWall = false;
    private bool _isGrabbingTheWall = false;
    
    // Player information
    private float _playerHalfHight = 0;
    private float _playerHalfWidth = 0;
    [SerializeField] private bool isInvincible = false;
    private int _ledgeLayer;
    
    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (_rigidBody2D == null)
        {
            throw new MissingComponentException("RigidBody2D missing on Player!");
        }

        if (_spriteRenderer == null)
        {
            throw new MissingComponentException("SpriteRenderer missing on Player!");
        }

        dashDistance -= 0.5f;
        _playerHalfHight = _spriteRenderer.bounds.size.y / 2 + .1f;
        _playerHalfWidth = 0.4f;
        _ledgeLayer = LayerMask.NameToLayer("Ledge");
        
        deathUI.SetActive(false);
    }

    private void OnEnable()
    {
        MovementControllerScript.OnPlayerHoldingWall += HandlePlayerReleasingTheWall;
        MovementControllerScript.OnPlayerMoveRight += HandlePlayerRightMovement;
        MovementControllerScript.OnPlayerMoveLeft += HandlePlayerLeftMovement;
        MovementControllerScript.OnPlayerJump += HandlePlayerJump;
        MovementControllerScript.OnPlayerDash += HandlePlayerDash;
        MovementControllerScript.OnPlayerStopMoving += HandleStopPlayerMovement;
        MovementControllerScript.OnPlayerSlidingDown += HandlePlayerSlidingDown;
        InputScript.OnFreezePlayer += HandleFreezePlayer;
    }

    private void OnDisable()
    {
        MovementControllerScript.OnPlayerHoldingWall -= HandlePlayerReleasingTheWall;
        MovementControllerScript.OnPlayerMoveRight -= HandlePlayerRightMovement;
        MovementControllerScript.OnPlayerMoveLeft -= HandlePlayerLeftMovement;
        MovementControllerScript.OnPlayerJump -= HandlePlayerJump;
        MovementControllerScript.OnPlayerDash -= HandlePlayerDash;
        MovementControllerScript.OnPlayerStopMoving -= HandleStopPlayerMovement;
        MovementControllerScript.OnPlayerSlidingDown -= HandlePlayerSlidingDown;
        InputScript.OnFreezePlayer -= HandleFreezePlayer;
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.DrawRay(transform.position, Vector2.down * (_playerHalfHight), Color.green);
        // Debug.DrawRay(transform.position, (_direction == -1 ? Vector2.left : Vector2.right) * dashDistance, Color.orange);
        Debug.DrawRay(transform.position, Vector2.right * (_playerHalfWidth + 0.05f), Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * (_playerHalfWidth + 0.05f), Color.blue);
        
        PlayerAnimationController();
        
        // Dash and Cool Down
        _dashTime -= Time.deltaTime;
        _dashCooldownTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_dashTime > 0) return;
        _rigidBody2D.gravityScale = gravityForce;
    }

    private void HandlePlayerReleasingTheWall(int direction)
    {
        if (!_isGrabbingTheWall) return;
        
        _isGrabbingTheWall = false;
        PlayerWallGrabAnimation?.Invoke(false);
        _rigidBody2D.constraints = RigidbodyConstraints2D.None;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidBody2D.linearDamping = 0;
        _rigidBody2D.linearVelocity = new Vector2(0.3f * (-direction), _rigidBody2D.linearVelocityY);
    }

    private void HandlePlayerSlidingDown(int direction)
    {
        if (!_isGrabbingTheWall) return;

        if (IsTouchingWall(direction))
        {
            StartSlidingEffect();
            _rigidBody2D.constraints = RigidbodyConstraints2D.None;
            _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rigidBody2D.linearDamping = dampingForce;
            return;
        }
        
        _isGrabbingTheWall = false;
        StopSlidingEffect();
        PlayerWallGrabAnimation?.Invoke(false);
        _rigidBody2D.constraints = RigidbodyConstraints2D.None;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rigidBody2D.linearDamping = 0;
    }

    private void HandleStopPlayerMovement()
    {
        // Decelerate the player if stopped moving
        _rigidBody2D.linearDamping = IsTouchingGround() || IsTouchingWall() ? dampingForce : 0;
    }
    
    private void HandlePlayerRightMovement()
    {
        Debug.DrawRay(transform.position, Vector2.right * (_playerHalfWidth + .1f), Color.blue);
        _rigidBody2D.transform.eulerAngles = new Vector3(0, 0, 0);
        _direction = 1;
        
        if (_dashTime > 0) return;

        if (!_isGrabbingTheWall && IsTouchingWall(_direction) && !IsTouchingGround())
        {
            WallGrabbing();
            return;
        }
        _rigidBody2D.linearVelocity = new Vector2(constantSpeed, _rigidBody2D.linearVelocityY);
    }

    private void HandlePlayerLeftMovement()
    {
        Debug.DrawRay(transform.position, Vector2.left * (_playerHalfWidth + .1f), Color.red);
        _rigidBody2D.transform.eulerAngles = new Vector3(0, 180, 0);
        _direction = -1;
        
        if (_dashTime > 0) return;

        if (!_isGrabbingTheWall && IsTouchingWall(_direction) && !IsTouchingGround())
        {
            WallGrabbing();
            return;
        }
        
        _rigidBody2D.linearVelocity = new Vector2(constantSpeed * -1, _rigidBody2D.linearVelocityY);
    }

    private void HandlePlayerJump()
    {
        if (!IsTouchingGround() && !_canJumpFromWall) return;
        
        _rigidBody2D.constraints = RigidbodyConstraints2D.None;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (_canJumpFromWall)
        {
            _canJumpFromWall = false;
            _isGrabbingTheWall = false;
            StopSlidingEffect();
            PlayerWallGrabAnimation?.Invoke(false);
        }

        _rigidBody2D.linearDamping = 0;
        _rigidBody2D.linearVelocity = new Vector2(_rigidBody2D.linearVelocityX, jumpForce);
    }

    private void HandlePlayerDash(int direction)
    {
        DashAbility();
        if (_dashTime < 0) return;

        DashUIAnimation?.Invoke(dashCooldown);
        
        var distance = CheckDashDistance(direction);
        _rigidBody2D.transform.position = new Vector3(
            _rigidBody2D.transform.position.x + (dashDistance - distance) * direction, 
            _rigidBody2D.transform.position.y, 
            _rigidBody2D.transform.position.z);
        _rigidBody2D.linearVelocity = new Vector2(_rigidBody2D.linearVelocityX, jumpForce / 2);
    }

    private void HandleFreezePlayer(bool isContinued)
    {
        if (isContinued)
        {
            _rigidBody2D.constraints = RigidbodyConstraints2D.None;
            _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (!IsTouchingGround())
                _rigidBody2D.linearVelocity = new Vector2(
                    _rigidBody2D.linearVelocityX, 
                    _rigidBody2D.linearVelocityY + 0.01f);
        }
        else
        {
            _rigidBody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        }
    }
    
    private bool IsTouchingGround() => Physics2D.Raycast(transform.position, Vector2.down, _playerHalfHight, tileLayer);

    private bool IsTouchingWall() => 
        Physics2D.Raycast(transform.position, Vector2.right, _playerHalfWidth, tileLayer) 
        || Physics2D.Raycast(transform.position, Vector2.left, _playerHalfWidth, tileLayer);
    
    private bool IsTouchingWall(int direction) => Physics2D.Raycast(transform.position, direction == 1 ? Vector2.right : Vector2.left, _playerHalfWidth, tileLayer);

    private float CheckDashDistance(int direction)
    {
        var hitWall = Physics2D.Raycast(transform.position, direction == 1 ? Vector2.right : Vector2.left, dashDistance, tileLayer);
        if (hitWall && hitWall.distance > 0) return dashDistance - hitWall.distance;
        return 0;
    }
    
    private void WallGrabbing()
    {
        _isGrabbingTheWall = true;
        _canJumpFromWall = true;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        PlayerWallGrabAnimation?.Invoke(true);
    }

    private void DashAbility()
    {
        if (_dashCooldownTime > 0) return;
        
        _dashCooldownTime = dashCooldown;
        _dashTime = dashDuration;
    }

    private void StartSlidingEffect()
    {
        slideGrindArm.Clear();
        slideGrindLeg.Clear();
            
        slideGrindArm.Play();
        slideGrindLeg.Play();
    }

    private void StopSlidingEffect()
    {
        slideGrindArm.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        slideGrindLeg.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void PlayerAnimationController()
    {
        PlayerRunAnimation?.Invoke(Mathf.Abs(_rigidBody2D.linearVelocityX));
        PlayerJumpAnimation?.Invoke(_rigidBody2D.linearVelocityY > 0.1f && !_isGrabbingTheWall);
        PlayerFallAnimation?.Invoke(_rigidBody2D.linearVelocityY < -0.1f && !_isGrabbingTheWall);
        PlayerDashAnimation?.Invoke(_dashTime > 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsTouchingGround())
        {
            _rigidBody2D.linearDamping = dampingForce;
        }

        if (collision.gameObject.CompareTag("Kill") && !isInvincible)
        {
            // trigger death animation and show death screen
            PlayerWallGrabAnimation?.Invoke(false);
            PlayerDeathAnimation?.Invoke();
            deathUI.SetActive(true);
            TurretFireControl?.Invoke();
            _rigidBody2D.constraints = RigidbodyConstraints2D.FreezePosition;
            MovementControllerScript.PlayerIsDead = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Door")) && IsTouchingGround() && _isGrabbingTheWall)
        {
            StopSlidingEffect();
            PlayerWallGrabAnimation?.Invoke(false);
            _isGrabbingTheWall = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Door"))
        {
            _rigidBody2D.linearDamping = 0;
            if (_isGrabbingTheWall)
            {
                _isGrabbingTheWall = false;
                StopSlidingEffect();
                PlayerWallGrabAnimation?.Invoke(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ledge"))
        {
            _canJumpFromWall = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ledge"))
        {
            _canJumpFromWall = false;
        }
    }
}
