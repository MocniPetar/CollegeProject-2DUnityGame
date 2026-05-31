using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerDetectionScript : MonoBehaviour
{
    [SerializeField] private GameObject fKeyInput;

    private void Awake()
    {
        fKeyInput.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // These need to be changed when adding or removing scenes
            if (gameObject.name == "PlayerSpawnPoint" && SceneManager.GetActiveScene().buildIndex - 1 < 1) return;
            if (gameObject.name == "PlayerNextLevelTrigger" && SceneManager.GetActiveScene().buildIndex + 1 > 5) return;
            
            fKeyInput.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            fKeyInput.SetActive(false);
        }
    }
}
