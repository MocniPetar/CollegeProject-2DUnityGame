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
    
    // The constant speed a player move when accelerated to that speed
    [SerializeField] private float constantSpeed = 8f;

    // The acceleration overtime to the const speed
    [SerializeField] private float accelerationFactor = 15f;
    [SerializeField] private float accelerationSpeed = 1f;
    
    // The deacceleration overtime until the player completely stops
    [SerializeField] private float dampingForce = 5f;
    
    public bool isTouchingLeftWall = false;
    public bool isTouchingRightWall = false;
    
    // ----------- Variables for horizontal movement ----------- //
    
    [SerializeField] private float jumpForce = 8f;
    private bool _isInAir = false;
    
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
        VerticalMovement();
        HorizontalMovement();
    }

    void VerticalMovement()
    {
        // ----------- Accelerating the player ----------- //

        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            _rigidBody2D.linearDamping = 0;
            isTouchingLeftWall = isTouchingRightWall = false;
        }

        // Moving the player right
        if (Keyboard.current.dKey.isPressed && !isTouchingRightWall)
        {
            if (accelerationSpeed < constantSpeed)
            {
                accelerationSpeed += accelerationFactor * Time.deltaTime;
                _rigidBody2D.linearVelocity = new Vector2(accelerationSpeed, _rigidBody2D.linearVelocityY);
            }
            else
            {
                _rigidBody2D.linearVelocity = new Vector2(constantSpeed, _rigidBody2D.linearVelocityY);
            }
        }
        
        // Moving the player left
        if (Keyboard.current.aKey.isPressed && !isTouchingLeftWall)
        {
            if (accelerationSpeed < constantSpeed)
            {
                // 10 -> accelerationFactor
                accelerationSpeed += accelerationFactor * Time.deltaTime;
                _rigidBody2D.linearVelocity = new Vector2(-accelerationSpeed, _rigidBody2D.linearVelocityY);
            }
            else
            {
                _rigidBody2D.linearVelocity = new Vector2(-constantSpeed, _rigidBody2D.linearVelocityY);
            }
        }
        
        // ----------- Decelerating the player ----------- //
        if (!_isInAir && (Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.aKey.wasReleasedThisFrame))
        {
            _rigidBody2D.linearDamping = dampingForce;
        }
        
        // This does not need fixing, it needs toning!!
        if (_rigidBody2D.linearVelocityX == 0 && accelerationSpeed > 1f)
            accelerationSpeed = 1f;
    }

    void HorizontalMovement()
    {
        // Used _rigidBody.linearVelocityY == 0f in this check but after adding more to the level (higher platform) the players linear velocity
        // went crazy. Do not know why so using _IsInAir is a quick fix (maybe a better fix)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !_isInAir)
        {
            _rigidBody2D.linearDamping = 0;
            _rigidBody2D.linearVelocity = new Vector2(_rigidBody2D.linearVelocityX, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            Debug.Log("The player is on the ground!");
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
            Debug.Log("The player is in the air!");
            _isInAir = true;
            _rigidBody2D.linearDamping = 0;
        }
    }
}
