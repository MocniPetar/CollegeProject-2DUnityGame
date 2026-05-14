using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementControllerScript : MonoBehaviour
{
    public static event Action OnPlayerHoldingWall;
    public static event Action OnPlayerMoveRight;
    public static event Action OnPlayerMoveLeft;
    public static event Action OnPlayerJump;
    public static event Action<int> OnPlayerDash;
    public static event Action OnPlayerStopMoving;
    public static event Action OnPlayerStartMoving;

    private int _direction = 0;
    public static bool PlayerIsDead = false;

    private void Awake()
    {
        PlayerIsDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerIsDead)
            KeyboardControl();
    }

    private void KeyboardControl()
    {
        // letting go of the wall
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            OnPlayerHoldingWall?.Invoke();
        }
        
        // moving left control
        if (Keyboard.current.aKey.isPressed)
        {
            _direction = -1;
            OnPlayerMoveLeft?.Invoke();
        }
        
        // moving right control
        if (Keyboard.current.dKey.isPressed)
        {
            _direction = 1;
            OnPlayerMoveRight?.Invoke();
        }
        
        // jump up control
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            OnPlayerJump?.Invoke();
        }
        
        // dash multi-directional control
        if (Keyboard.current.shiftKey.wasPressedThisFrame)
        {
            OnPlayerDash?.Invoke(_direction);
        }
        
        // trigger when player started moving
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            OnPlayerStartMoving?.Invoke();
            OnPlayerHoldingWall?.Invoke();
        }
        
        // trigger when player stopped moving
        if (Keyboard.current.aKey.wasReleasedThisFrame || Keyboard.current.dKey.wasReleasedThisFrame)
        {
            OnPlayerStopMoving?.Invoke();
        }
    }
}
