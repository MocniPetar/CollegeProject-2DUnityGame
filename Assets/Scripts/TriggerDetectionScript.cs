using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerDetectionScript : MonoBehaviour
{
    [SerializeField] private GameObject fKeyInput;
    public static bool IsAllowedToNextLevel { get; set; } = true;
    
    private void Awake()
    {
        fKeyInput.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        // These need to be changed when adding or removing scenes
        if (!IsAllowedToNextLevel || (SceneManager.GetActiveScene().buildIndex + 1 > 9 && !InputScript.IsSelectedLevel)) return;

        fKeyInput.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            fKeyInput.SetActive(false);
        }
    }
}
