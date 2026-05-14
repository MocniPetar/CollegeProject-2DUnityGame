using System;
using UnityEngine;

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
