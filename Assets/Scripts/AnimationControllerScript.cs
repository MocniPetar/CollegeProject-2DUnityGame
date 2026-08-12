using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class AnimationControllerScript : MonoBehaviour
{
    // Game object animators
    [CanBeNull] public Animator playerAnimator;
    [CanBeNull] public Animator turretAnimator;
    [CanBeNull] public Animator itemAnimator;
    [CanBeNull] public Animator sceneTransitionAnimator;
    [CanBeNull] public Animator dashAnimator;
    
    // Animation parameters ID's
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int CanGrabWall = Animator.StringToHash("canGrabWall");
    private static readonly int Start = Animator.StringToHash("Start");
    private static readonly int IsDead = Animator.StringToHash("IsDead");
    private static readonly int DashIsEmpty = Animator.StringToHash("DashIsEmpty");
    private static readonly int DashAnimationShow = Animator.StringToHash("DashAnimationShow");

    private void OnEnable()
    {
        // Player animations
        PlayerScript.PlayerRunAnimation += HandlePlayerRunAnimation;
        PlayerScript.PlayerJumpAnimation += HandlePlayerJumpAnimation;
        PlayerScript.PlayerDashAnimation += HandlePlayerDashAnimation;
        PlayerScript.PlayerFallAnimation += HandlePlayerFallAnimation;
        PlayerScript.PlayerWallGrabAnimation += HandlePlayerWallGrabAnimation;
        PlayerScript.PlayerDeathAnimation += HandlePlayerDeathAnimation;
        PlayerScript.DashUIAnimation += HandleDashUIAnimation;
        
        // Level loading animations
        LevelLoaderScript.TransitionAnimation += HandleTransitionAnimation;
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
        PlayerScript.DashUIAnimation -= HandleDashUIAnimation;
        
        // Level loading animations
        LevelLoaderScript.TransitionAnimation -= HandleTransitionAnimation;
    }

    private void HandlePlayerRunAnimation(float playerSpeed)
    {
        playerAnimator?.SetFloat(Speed, playerSpeed);
    }

    private void HandlePlayerJumpAnimation(bool isJumping)
    {
        playerAnimator?.SetBool(IsJumping, isJumping);
    }

    private void HandleDashUIAnimation(float dashCooldown)
    {
        StartCoroutine(DashAnimation(dashCooldown));
    }

    private void HandlePlayerDashAnimation(bool isDashing)
    {
        playerAnimator?.SetBool(IsDashing, isDashing);
    }

    private void HandlePlayerFallAnimation(bool isFalling)
    {
        playerAnimator?.SetBool(IsFalling, isFalling);
    }

    private void HandlePlayerWallGrabAnimation(bool isWallGrabbing)
    {
        playerAnimator?.SetBool(CanGrabWall, isWallGrabbing);
    }
    
    private void HandlePlayerDeathAnimation()
    {
        playerAnimator?.SetTrigger(IsDead);
    }

    private void HandleTransitionAnimation()
    {
        sceneTransitionAnimator?.SetTrigger(Start);
    }
    
    private IEnumerator DashAnimation(float dashCooldown)
    {
        dashAnimator?.SetBool(DashAnimationShow, true);
        
        yield return new WaitForSeconds((float)(dashCooldown - dashCooldown*0.1));

        dashAnimator?.SetTrigger(DashIsEmpty);
        yield return new WaitForSeconds((float)(dashCooldown*0.1));
        
        dashAnimator?.SetBool(DashAnimationShow, false);
    }
}
