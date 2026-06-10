using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    [SerializeField] private GameObject transition;
    public static event Action TransitionAnimation;
    public float transitionTime = 1f;

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
        if (SceneManager.GetActiveScene().buildIndex == 5) return;
        
        // Need to use StartCoroutine() to tell Unity that this function call is of type IEnumerator and that it is
        // going to wait (it will essential pause all other code execution)
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private void LoadPreviousLevel()
    {
        // This needs to be changed when adding or removing scenes
        if (SceneManager.GetActiveScene().buildIndex == 0) return;
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex - 1));
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

    public void RestartLevel()
    {
        // MovementControllerScript.PlayerIsDead = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
