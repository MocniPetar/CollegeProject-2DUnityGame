using System;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public static event Action OpenDoorOnPickUp;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Picked up the item");
        
        // Update the database - change the state to picked up
        // Update the player UI - there will be in some corner a counter to show the player the amount of items they picked up
        // Play a sound - when sound gets added play one when the item is picked up
        
        // Store a bool in the DB which is used to know if a level has a door or not. If it has than trigger the OpenDoorOnPickUp action
        OpenDoorOnPickUp?.Invoke();
        Destroy(gameObject);
    }
}
