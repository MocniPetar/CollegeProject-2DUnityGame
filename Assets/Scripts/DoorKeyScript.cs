using System;
using UnityEngine;

public class DoorKeyScript : MonoBehaviour
{
    public static event Action OpenDoorOnPickUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Play a sound - when sound gets added play one when the item is picked up

        if (!other.gameObject.CompareTag("Player")) return;
        OpenDoorOnPickUp?.Invoke();
        Destroy(gameObject);
    }
}
