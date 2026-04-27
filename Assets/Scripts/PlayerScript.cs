using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerScript : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");

    // Player components
    private Rigidbody2D _rigidBody2D;
    public Animator animator;
    
    // Player movement variables
    private int _playerDirection = 1;
    
    // The constant speed a player moves when accelerated to that speed
    [SerializeField] private float constantSpeed;
    
    // The acceleration overtime to the const speed
    [SerializeField] private float accelerationFactor;
    [SerializeField] private float accelerationSpeed;
    
    // The deacceleration overtime until the player completely stops
    [SerializeField] private float dampingForce;
    
    // Dashing
    //      - when the player dashes it starts a timer for the dash duration
    //      - after the dash duration finishes the timer for the cooldown is started
    //      - every timer is subtracted by Time.deltaTime
    private bool _isDashing = false;
    private int _dashDirection = 0;
    private float _dashTime;
    private float _dashCooldownTime;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashJump;
    [SerializeField] private float dashCooldown;

    [SerializeField] private float jumpForce = 8f;
    private bool _jumped = false;
    private bool _isInAir = true;
    
    // Player collision variables
    public bool isTouchingLeftWall = false;
    public bool isTouchingRightWall = false;
    
    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        if (_rigidBody2D == null)
        {
            Debug.LogError("RigidBody2D missing on Player!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputDetection();
        AnimationPlayer();
        
        // Dash and Cool Down
        _dashTime -= Time.deltaTime;
        _dashCooldownTime -= Time.deltaTime;
    }

    void InputDetection()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            _rigidBody2D.linearDamping = 0;
            if (_dashTime > 0)
                _rigidBody2D.gravityScale = 0;
            else
                _rigidBody2D.gravityScale = 2;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            _playerDirection = -1;
            PlayerAngle();
            VerticalMovement();
        }

        if (Keyboard.current.dKey.isPressed)
        {
            _playerDirection = 1;
            PlayerAngle();
            VerticalMovement();
        }

        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            DashAbility();
            VerticalMovement();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !_isInAir)
        {
            HorizontalMovement();
        }
    }

    void DashAbility()
    {
        if (_dashCooldownTime < 0)
        {
            _dashCooldownTime = dashCooldown;
            _dashTime = dashDuration;
        }
    }

    void PlayerAngle()
    {
        if (_playerDirection == -1)
            _rigidBody2D.transform.eulerAngles = new Vector3(0, 180, 0);
        else if (_playerDirection == 1)
            _rigidBody2D.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void VerticalMovement()
    {
        // ----------- Accelerating the player ----------- //

        if (_dashTime > 0)
        {
            _rigidBody2D.linearVelocity = new Vector2(dashSpeed * _playerDirection, 0);
        }
        else
        {
            if (accelerationSpeed < constantSpeed)
            {
                accelerationSpeed += accelerationFactor * Time.deltaTime;
                _rigidBody2D.linearVelocity = new Vector2(accelerationSpeed * _playerDirection, _rigidBody2D.linearVelocityY);
            }
            else
            {
                _rigidBody2D.linearVelocity = new Vector2(constantSpeed * _playerDirection, _rigidBody2D.linearVelocityY);
            }
        }

        // ----------- Decelerating the player ----------- //
        
        if (!_isInAir && _playerDirection != 0)
        {
            _rigidBody2D.linearDamping = dampingForce;
        }
        
        // This does not need fixing, it needs toning!!
        if (_rigidBody2D.linearVelocityX == 0 && accelerationSpeed > 1f)
            accelerationSpeed = 1f;
    }

    void HorizontalMovement()
    {
        _rigidBody2D.linearDamping = 0;
        _rigidBody2D.linearVelocity = new Vector2(_rigidBody2D.linearVelocityX, jumpForce);
    }

    void AnimationPlayer()
    {
        animator.SetFloat(Speed, Mathf.Abs(_rigidBody2D.linearVelocityX));
        animator.SetBool(IsDashing, _dashTime > 0);
        animator.SetBool(IsJumping, _isInAir && _rigidBody2D.linearVelocityY > 0);
        animator.SetBool(IsFalling, _isInAir && _rigidBody2D.linearVelocityY < -0.1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isInAir = false;
            _rigidBody2D.linearDamping = dampingForce;
        }

        // if (collision.gameObject.layer == 6)
        // {
        //     if (Keyboard.current.aKey.isPressed)
        //         isTouchingLeftWall = true;
        //     
        //     if(Keyboard.current.dKey.isPressed)
        //         isTouchingRightWall = true;
        //     
        //     _rigidBody2D.linearVelocity = new Vector2(0, _rigidBody2D.linearVelocityY);
        // }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isInAir = true;
            _rigidBody2D.linearDamping = 0;
        }
    }
}
