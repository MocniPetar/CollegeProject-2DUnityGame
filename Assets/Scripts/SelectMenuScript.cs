using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectMenuScript : MonoBehaviour
{
    public static Action<int> LoadSelected;

    [SerializeField] private Button previousLevelButton;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private List<GameObject> levels;
    private int inViewLevel;

    private void Awake()
    {
        inViewLevel = 0;
        InputScript.IsSelectedLevel = false;
    }

    private void Update()
    {
        nextLevelButton.interactable = inViewLevel < levels.Count - 1;
        previousLevelButton.interactable = inViewLevel != 0;
    }
    
    public void ShowNextLevel()
    {
        if (inViewLevel >= levels.Count - 1) return;
        
        levels[inViewLevel].SetActive(false);
        inViewLevel++;
        levels[inViewLevel].SetActive(true);
    }
    
    public void ShowPreviousLevel()
    {
        if (inViewLevel == 0) return;
        
        levels[inViewLevel].SetActive(false);
        inViewLevel--;
        levels[inViewLevel].SetActive(true);
    }

    public void LoadSelectedLevel()
    {
        TriggerDetectionScript.IsAllowedToNextLevel = true;
        InputScript.IsSelectedLevel = true;
        LoadSelected?.Invoke(inViewLevel + 2);
    }
}
