using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputScript : MonoBehaviour
{
    public static event Action LoadNextLevel;
    public static event Action LoadPreviousLevel;
    
    [SerializeField] private GameObject triggerOne;
    [SerializeField] private GameObject triggerTwo;

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
                LoadNextLevel?.Invoke();
            }
        }
    }
}
