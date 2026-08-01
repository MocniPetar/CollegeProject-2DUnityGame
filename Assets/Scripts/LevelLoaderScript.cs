using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    [SerializeField] private GameObject transition;
    [SerializeField] [CanBeNull] private GameObject deathUI;
    [SerializeField] [CanBeNull] private GameObject pauseMenu;
    public static event Action TransitionAnimation;
    public float transitionTime = 1.5f;

    private void Awake()
    {
        transition.SetActive(true);
    }

    private void OnEnable()
    {
        InputScript.LoadNextLevel += LoadNextLevel;
        InputScript.LoadPreviousLevel += LoadPreviousLevel;
    }

    private void OnDisable()
    {
        InputScript.LoadNextLevel -= LoadNextLevel;
        InputScript.LoadPreviousLevel -= LoadPreviousLevel;
    }

    private void LoadNextLevel()
    {                                               
        // This needs to be changed when adding or removing scenes
        if (SceneManager.GetActiveScene().buildIndex == 6) return;
        
        // Need to use StartCoroutine() to tell Unity that this function call is of type IEnumerator and that it is
        // going to wait (it will essential pause all other code execution)
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private void LoadPreviousLevel()
    {
        // This needs to be changed when adding or removing scenes
        if (SceneManager.GetActiveScene().buildIndex == 1) return;
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex - 1));
    }
    
    public void GoToMainMenu()
    {
        if (deathUI != null && deathUI.activeSelf)
            deathUI.SetActive(false);
        
        if (pauseMenu != null && pauseMenu.activeSelf)
            pauseMenu.SetActive(false);
        
        StartCoroutine(LoadLevel(0));
    }

    // Use this when you want to delay something - in this situation we want to start the transition animation and wait for 1 second (which is the animation duration),
    // after that wait we call the scene manager and change the scene
    IEnumerator LoadLevel(int levelIndex)
    {
        // Play animation
        TransitionAnimation?.Invoke();
        
        //Wait
        yield return new WaitForSeconds(transitionTime);
        
        //Load scene
        SceneManager.LoadScene(levelIndex);
    }
    
    public void StartFirstLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void RestartLevel()
    {
        // MovementControllerScript.PlayerIsDead = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitGame()
    {
        // Closes the built game application
        Application.Quit();

        // Output a message to the console so you can test it inside the Unity Editor
        #if UNITY_EDITOR
                Debug.Log("Game Exited!");
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
