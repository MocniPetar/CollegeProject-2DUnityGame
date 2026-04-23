using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerScript : MonoBehaviour
{
    // Player components
    private Rigidbody2D _rigidBody2D;
    
    /*
     *  ----------- Variables for vertical movement ----------- *
     *  - left and right move speed
     *  - dash speed
     */

    private int _playerDirection = 0;
    private bool _pressedSpaceKey = false;
    
    // The constant speed a player move when accelerated to that speed
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
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float _dashTime;

    [SerializeField] private float dashCooldown;
    private float _dashCooldownTime;
    
    public bool isTouchingLeftWall = false;
    public bool isTouchingRightWall = false;
    
    // ----------- Variables for horizontal movement ----------- //
    
    [SerializeField] private float jumpForce = 8f;
    private bool _isInAir = true;
    
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
        KeyDetection();
        VerticalLinearVelocity();
        HorizontalLinearVelocity();
        
        // Dash and Cool Down
        _dashTime -= Time.deltaTime;
        _dashCooldownTime -= Time.deltaTime;
    }

    void KeyDetection()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            _playerDirection = 0;
            _rigidBody2D.linearDamping = 0;
            isTouchingLeftWall = isTouchingRightWall = false;
        }

        if (Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.aKey.wasReleasedThisFrame)
            _playerDirection = 0;

        if (Keyboard.current.aKey.isPressed && !isTouchingLeftWall)
            _playerDirection = -1;
        
        if (Keyboard.current.dKey.isPressed && !isTouchingRightWall)
            _playerDirection = 1;

        if (Keyboard.current.shiftKey.wasPressedThisFrame)
            DashAbility();

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !_isInAir)
            _pressedSpaceKey = true;
    }

    void DashAbility()
    {
        if (_dashCooldownTime < 0)
        {
            _dashCooldownTime = dashCooldown;
            _dashTime = dashDuration;
        }
    }

    void VerticalLinearVelocity()
    {
        // ----------- Accelerating the player ----------- //

        // Moving the player left or right
        if (_playerDirection != 0)
        {
            if (_dashTime > 0)
            {
                _rigidBody2D.linearVelocity = new Vector2(dashSpeed * _playerDirection * 2f, 0);
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

    void HorizontalLinearVelocity()
    {
        if (_pressedSpaceKey)
        {
            _rigidBody2D.linearDamping = 0;
            _rigidBody2D.linearVelocity = new Vector2(_rigidBody2D.linearVelocityX, jumpForce);
            _pressedSpaceKey = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isInAir = false;
            _rigidBody2D.linearDamping = dampingForce;
        }

        if (collision.gameObject.layer == 6)
        {
            if (Keyboard.current.aKey.isPressed)
                isTouchingLeftWall = true;
            
            if(Keyboard.current.dKey.isPressed)
                isTouchingRightWall = true;
            
            _rigidBody2D.linearVelocity = new Vector2(0, _rigidBody2D.linearVelocityY);
        }
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
