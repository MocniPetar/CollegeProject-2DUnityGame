using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerScript : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int CanGrabWall = Animator.StringToHash("canGrabWall");
    
    // Player Animation Actions
    public static event Action<float> PlayerRunAnimation;
    public static event Action<bool> PlayerJumpAnimation;
    public static event Action<bool> PlayerWallGrabAnimation;
    public static event Action<bool> PlayerFallAnimation;
    public static event Action<bool> PlayerDashAnimation;
    public static event Action PlayerDeathAnimation;

    // Player components
    private Rigidbody2D _rigidBody2D;
    private SpriteRenderer _spriteRenderer;
    public Animator animator;
    
    // Other components
    [SerializeField] private GameObject deathUI;
    
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
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashJump;
    [SerializeField] private float dashCooldown;

    [SerializeField] private float jumpForce;
    private bool _canJumpFromWall = false;
    private bool _isGrabbingTheWall = false;
    
    // Player information
    private float _playerHalfHight = 0;
    private float _playerHalfWidth = 0;
    
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
        
        _playerHalfHight = _spriteRenderer.bounds.size.y / 2;
        _playerHalfWidth =  _spriteRenderer.bounds.size.x / 2;
        
        deathUI.SetActive(false);
    }

    private void OnEnable()
    {
        MovementControllerScript.OnPlayerHoldingWall += HandlePlayerReleasingTheWall;
        MovementControllerScript.OnPlayerMoveRight += HandlePlayerRightMovement;
        MovementControllerScript.OnPlayerMoveLeft += HandlePlayerLeftMovement;
        MovementControllerScript.OnPlayerJump += HandlePlayerJump;
        MovementControllerScript.OnPlayerDash += HandlePlayerDash;
        MovementControllerScript.OnPlayerStartMoving += HandleStartPlayerMovement;
        MovementControllerScript.OnPlayerStopMoving += HandleStopPlayerMovement;
    }

    private void OnDisable()
    {
        MovementControllerScript.OnPlayerHoldingWall -= HandlePlayerReleasingTheWall;
        MovementControllerScript.OnPlayerMoveRight -= HandlePlayerRightMovement;
        MovementControllerScript.OnPlayerMoveLeft -= HandlePlayerLeftMovement;
        MovementControllerScript.OnPlayerJump -= HandlePlayerJump;
        MovementControllerScript.OnPlayerDash -= HandlePlayerDash;
        MovementControllerScript.OnPlayerStartMoving -= HandleStartPlayerMovement;
        MovementControllerScript.OnPlayerStopMoving -= HandleStopPlayerMovement;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Vector2.down * (_playerHalfHight + .1f), Color.green);
        
        PlayerAnimationController();
        
        // Dash and Cool Down
        _dashTime -= Time.deltaTime;
        _dashCooldownTime -= Time.deltaTime;

        if (_dashTime > 0) return;
        _rigidBody2D.gravityScale = gravityForce;
    }

    void HandlePlayerReleasingTheWall()
    {
        if (!_isGrabbingTheWall) return;
        
        _isGrabbingTheWall = false;
        PlayerWallGrabAnimation?.Invoke(false);
        _rigidBody2D.constraints = RigidbodyConstraints2D.None;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    void HandleStartPlayerMovement()
    {
        PlayerRunAnimation?.Invoke(1f);
    }

    void HandleStopPlayerMovement()
    {
        // Decelerate the player if stopped moving
        _rigidBody2D.linearDamping = IsTouchingGround() ? dampingForce : 0;
        PlayerRunAnimation?.Invoke(0);
    }
    
    void HandlePlayerRightMovement()
    {
        Debug.DrawRay(transform.position, Vector2.right * (_playerHalfWidth - .1f), Color.blue);
        _rigidBody2D.transform.eulerAngles = new Vector3(0, 0, 0);
        
        if (_dashTime > 0) return;
        _rigidBody2D.linearVelocity = new Vector2(constantSpeed, _rigidBody2D.linearVelocityY);

        if (!IsTouchingRightSide() || IsTouchingGround()) return;
        WallGrabbing();
    }

    void HandlePlayerLeftMovement()
    {
        Debug.DrawRay(transform.position, Vector2.left * (_playerHalfWidth - .1f), Color.red);
        _rigidBody2D.transform.eulerAngles = new Vector3(0, 180, 0);
        
        if (_dashTime > 0) return;
        _rigidBody2D.linearVelocity = new Vector2(constantSpeed * -1, _rigidBody2D.linearVelocityY);

        if (!IsTouchingLeftSide() || IsTouchingGround()) return;
        WallGrabbing();
    }

    void HandlePlayerJump()
    {
        if (!IsTouchingGround() && !_canJumpFromWall) return;
        
        _rigidBody2D.constraints = RigidbodyConstraints2D.None;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (_canJumpFromWall)
        {
            _canJumpFromWall = false;
            _isGrabbingTheWall = false;
            PlayerWallGrabAnimation?.Invoke(false);
        }

        _rigidBody2D.linearDamping = 0;
        _rigidBody2D.linearVelocity = new Vector2(_rigidBody2D.linearVelocityX, jumpForce);
    }

    void HandlePlayerDash(int direction)
    {
        DashAbility();
        if (_dashTime < 0) return;
        _rigidBody2D.gravityScale = 0;
        _rigidBody2D.linearDamping = 0;
        _rigidBody2D.linearVelocity = new Vector2(dashSpeed * direction, 0);
    }

    private bool IsTouchingLeftSide() => Physics2D.Raycast(transform.position, Vector2.left, _playerHalfWidth - 0.05f, LayerMask.GetMask("Ground"));
    private bool IsTouchingRightSide() => Physics2D.Raycast(transform.position, Vector2.right, _playerHalfWidth - 0.05f, LayerMask.GetMask("Ground"));
    private bool IsTouchingGround() => Physics2D.Raycast(transform.position, Vector2.down, _playerHalfHight + .01f,LayerMask.GetMask("Ground"));

    private float CheckDashPossibleDistance(int direction)
    {
        float distance = 0;
        
        // Check for ground
        if (Physics2D.Raycast(transform.position, direction == 1 ? Vector2.right : Vector2.left,
                _playerHalfWidth - 0.05f, LayerMask.GetMask("Ground")))
        {
            // Calculate the distance
        }
        
        // Check for kill ground
        if (Physics2D.Raycast(transform.position, direction == 1 ? Vector2.right : Vector2.left,
                _playerHalfWidth - 0.05f, LayerMask.GetMask("Kill")))
        {
            // Calculate the distance
        }

        return distance;
    }
    
    private void WallGrabbing()
    {
        _isGrabbingTheWall = true;
        _canJumpFromWall = true;
        _rigidBody2D.constraints = RigidbodyConstraints2D.FreezePosition;
        PlayerWallGrabAnimation?.Invoke(true);
    }

    void DashAbility()
    {
        if (_dashCooldownTime > 0) return;
        
        _dashCooldownTime = dashCooldown;
        _dashTime = dashDuration;
    }

    void PlayerAnimationController()
    {
        PlayerJumpAnimation?.Invoke(_rigidBody2D.linearVelocityY > 0.1f);
        PlayerFallAnimation?.Invoke(_rigidBody2D.linearVelocityY < -0.1f);
        PlayerDashAnimation?.Invoke(_dashTime > 0);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsTouchingGround())
        {
            _rigidBody2D.linearDamping = dampingForce;
        }

        if (collision.gameObject.layer == 7)
        {
            // trigger death animation and show death screen
            PlayerDeathAnimation?.Invoke();
            deathUI.SetActive(true);
            _rigidBody2D.constraints = RigidbodyConstraints2D.FreezePosition;
            MovementControllerScript.PlayerIsDead = true;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _rigidBody2D.linearDamping = 0;
        }
    }
}
