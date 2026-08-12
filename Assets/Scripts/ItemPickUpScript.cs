using System;
using UnityEngine;

public class ItemPickUpScript : MonoBehaviour
{
    private static int _itemsPickedUp;

    private void Awake()
    {
        _itemsPickedUp = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        // Control the "next level trigger from here":
        /*
         * Using an action from ItemPickUpScript invoke a change in two scripts that will allow the player to go to the
         * next level after he picked up all of the items in the scene
         */
        
        _itemsPickedUp++;
        if (_itemsPickedUp == SpawningSystemScript.NumberOfWords)
            TriggerDetectionScript.IsAllowedToNextLevel = true;

        Destroy(gameObject);
    }
}
