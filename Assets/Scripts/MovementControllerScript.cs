using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementControllerScript : MonoBehaviour
{
    public static event Action<int> OnPlayerHoldingWall;
    public static event Action OnPlayerMoveRight;
    public static event Action OnPlayerMoveLeft;
    public static event Action OnPlayerJump;
    public static event Action<int> OnPlayerDash;
    public static event Action OnPlayerStopMoving;
    public static event Action<int> OnPlayerSlidingDown;

    private int _direction = 1;
    public static bool PlayerIsDead = false;

    private void Awake()
    {
        PlayerIsDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerIsDead)
            KeyboardControlUsingUpdate();
    }

    private void FixedUpdate()
    {
        if (!PlayerIsDead)
            KeyboardControlUsingFixedUpdate();
    }

    private void KeyboardControlUsingUpdate()
    {
        // jump up control
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnPlayerJump?.Invoke();
        }
        
        // letting go of the wall
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            OnPlayerHoldingWall?.Invoke(_direction);
        }
        
        // trigger when player started moving left
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            _direction = -1;
            OnPlayerSlidingDown?.Invoke(_direction);
        }

        // trigger when player started moving right
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            _direction = 1;
            OnPlayerSlidingDown?.Invoke(_direction);
        }
        
        // dash multi-directional control
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            OnPlayerDash?.Invoke(_direction);
        }
        
        // trigger when player stopped moving
        if (Keyboard.current.aKey.wasReleasedThisFrame || Keyboard.current.dKey.wasReleasedThisFrame)
        {
            OnPlayerStopMoving?.Invoke();
        }
    }
    
    private void KeyboardControlUsingFixedUpdate()
    {
        // moving left control
        if (Keyboard.current.aKey.isPressed)
        {
            OnPlayerMoveLeft?.Invoke();
        }
        
        // moving right control
        if (Keyboard.current.dKey.isPressed)
        {
            OnPlayerMoveRight?.Invoke();
        }
    }
}
