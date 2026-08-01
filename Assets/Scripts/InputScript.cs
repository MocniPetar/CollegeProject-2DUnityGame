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
    
    [SerializeField] [CanBeNull] private GameObject UI;
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject pauseMenu;
    public static event Action LoadNextLevel;
    public static event Action LoadPreviousLevel;
    
    [SerializeField] private GameObject triggerOne;
    [SerializeField] private GameObject triggerTwo;

    private void Awake()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        OnInput();
    }

    void OnInput()
    {
        // F key - interacted with the spawning/despawning platform
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (triggerOne.transform.GetChild(0).gameObject.activeSelf)
            {
                LoadPreviousLevel?.Invoke();
            }
            
            if (triggerTwo.transform.GetChild(0).gameObject.activeSelf)
            {
                if (UI != null && UI.activeSelf)
                    UI.SetActive(false);
                LoadNextLevel?.Invoke();
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame && !deathUI.activeSelf)
        {
            ContinueGame();
        }
    }

    public void ContinueGame()
    {
        if (FindAnyObjectByType<TurretScript>())
        {
            OnFreezeBullet?.Invoke(TurretFireControl.Invoke());
        }
        MovementControllerScript.PlayerIsDead = !MovementControllerScript.PlayerIsDead;
        OnFreezePlayer?.Invoke(!MovementControllerScript.PlayerIsDead);
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
}
