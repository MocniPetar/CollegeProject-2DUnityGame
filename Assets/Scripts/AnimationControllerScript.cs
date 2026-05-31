using System;
using System.Collections;
using UnityEngine;

public class AnimationControllerScript : MonoBehaviour
{
    // Game object animators
    public Animator playerAnimator;
    public Animator turretAnimator;
    public Animator itemAnimator;
    public Animator sceneTransitionAnimator;
    
    // Animation parameters ID's
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int CanGrabWall = Animator.StringToHash("canGrabWall");
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int Fire = Animator.StringToHash("Fire");

    private void OnEnable()
    {
        // Player animations
        PlayerScript.PlayerRunAnimation += HandlePlayerRunAnimation;
        PlayerScript.PlayerJumpAnimation += HandlePlayerJumpAnimation;
        PlayerScript.PlayerDashAnimation += HandlePlayerDashAnimation;
        PlayerScript.PlayerFallAnimation += HandlePlayerFallAnimation;
        PlayerScript.PlayerWallGrabAnimation += HandlePlayerWallGrabAnimation;
        PlayerScript.PlayerDeathAnimation += HandlePlayerDeathAnimation;
        
        // Level loading animations
        LevelLoaderScript.TransitionAnimation += HandleTransitionAnimation;
        
        // Turret animations
        TurretScript.FireTurretAnimation += HandleTurretFiringAnimation;
    }

    private void OnDisable()
    {
        // Player animations
        PlayerScript.PlayerRunAnimation -= HandlePlayerRunAnimation;
        PlayerScript.PlayerJumpAnimation -= HandlePlayerJumpAnimation;
        PlayerScript.PlayerDashAnimation -= HandlePlayerDashAnimation;
        PlayerScript.PlayerFallAnimation -= HandlePlayerFallAnimation;
        PlayerScript.PlayerWallGrabAnimation -= HandlePlayerWallGrabAnimation;
        PlayerScript.PlayerDeathAnimation -= HandlePlayerDeathAnimation;
        
        // Level loading animations
        LevelLoaderScript.TransitionAnimation -= HandleTransitionAnimation;
        
        // Turret animations
        TurretScript.FireTurretAnimation -= HandleTurretFiringAnimation;
    }

    private void HandlePlayerRunAnimation(float playerSpeed)
    {
        playerAnimator.SetFloat(Speed, playerSpeed);
    }

    private void HandlePlayerJumpAnimation(bool isJumping)
    {
        playerAnimator.SetBool(IsJumping, isJumping);
    }

    private void HandlePlayerDashAnimation(bool isDashing)
    {
        playerAnimator.SetBool(IsDashing, isDashing);
    }

    private void HandlePlayerFallAnimation(bool isFalling)
    {
        playerAnimator.SetBool(IsFalling, isFalling);
    }

    private void HandlePlayerWallGrabAnimation(bool isWallGrabbing)
    {
        playerAnimator.SetBool(CanGrabWall, isWallGrabbing);
    }
    
    private void HandlePlayerDeathAnimation()
    {
        playerAnimator.SetTrigger(IsDead);
    }

    private void HandleTransitionAnimation()
    {
        sceneTransitionAnimator.SetTrigger(Start);
    }

    private void HandleTurretFiringAnimation()
    {
        turretAnimator.SetTrigger(Fire);
    }
}
