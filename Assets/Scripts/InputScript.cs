using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour
{
    // Input Actions
    public static event Action<bool> OnFreezePlayer;
    public static event Action<bool> OnFreezeBullet;
    public static event Func<bool> TurretFireControl;
    public static event Action GoToSelectMenu;
    public static event Action LoadNextLevel;
    public static bool IsSelectedLevel { get; set; }

    [SerializeField] [CanBeNull] private GameObject UI;
    [SerializeField] [CanBeNull] private GameObject deathUI;
    [SerializeField] [CanBeNull] private GameObject pauseMenu;
    [SerializeField] [CanBeNull] private GameObject trigger;

    private void Awake()
    {
        if (pauseMenu != null)
            pauseMenu?.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        OnInput();
    }

    private void OnInput()
    {
        // F key - interacted with the spawning/despawning platform
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (trigger && trigger.transform.GetChild(0).gameObject.activeSelf)
            {
                if (UI && UI.activeSelf)
                {
                    UI.SetActive(false);
                }

                if (IsSelectedLevel)
                {
                    GoToSelectMenu?.Invoke();
                }
                else
                {
                    LoadNextLevel?.Invoke();
                }
            }
        }

        // Esc key - to open the pause menu
        if (Keyboard.current.escapeKey.wasPressedThisFrame && deathUI != null && !deathUI.activeSelf)
        {
            ContinueGame();
        }
    }

    public void ContinueGame()
    {
        if (FindAnyObjectByType<TurretScript>())
        {
            bool? turretShootingState = TurretFireControl?.Invoke();

            if (turretShootingState.HasValue)
            {
                OnFreezeBullet?.Invoke(turretShootingState.Value);
            }
        }
        MovementControllerScript.PlayerIsDead = !MovementControllerScript.PlayerIsDead;
        OnFreezePlayer?.Invoke(!MovementControllerScript.PlayerIsDead);
        if (pauseMenu != null)
            pauseMenu?.SetActive(!pauseMenu.activeSelf);
    }
}
