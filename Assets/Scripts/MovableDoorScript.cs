using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovableDoorScript : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private string direction;
    [SerializeField] private float speed;
    [SerializeField] private float maxAmount;
    private float _counter;
    private bool _openDoor;

    private void Awake()
    {
        _counter = 0;
        _openDoor = false;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null)
        {
            throw new MissingComponentException("Rigidbody2D component is missing");
        }
    }

    private void OnEnable()
    {
        ItemScript.OpenDoorOnPickUp += HandleDoorOpening;
    }

    private void OnDisable()
    {
        ItemScript.OpenDoorOnPickUp -= HandleDoorOpening;
    }

    private void Update()
    {
        if (_counter > maxAmount)
            _openDoor = false;
    }
    
    void FixedUpdate()
    {
        if (!_openDoor) return;
        switch (direction)
        {
            case "left":
                _rigidbody2D.transform.position = new Vector3(transform.position.x + (speed * -1 * Time.deltaTime),
                    transform.position.y,
                    transform.position.z);
                break;
            case "right":
                _rigidbody2D.transform.position = new Vector3(transform.position.x + (speed * 1 * Time.deltaTime),
                    transform.position.y,
                    transform.position.z);
                break;
            case "up":
                _rigidbody2D.transform.position = new Vector3(transform.position.x,
                    transform.position.y + (speed * 1 * Time.deltaTime),
                    transform.position.z);
                break;
            case "down":
                _rigidbody2D.transform.position = new Vector3(transform.position.x,
                    transform.position.y + (speed * -1 * Time.deltaTime),
                    transform.position.z);
                break;
        }
        
        _counter += speed * Time.deltaTime;
    }

    private void HandleDoorOpening()
    {
        _openDoor = true;
    }
}
