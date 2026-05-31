using System;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float speed;
    [SerializeField] private int direction;
    [SerializeField] private bool horizontalMovement;
    [SerializeField] private float maxAmount;
    private float _counter;

    private void Awake()
    {
        _counter = 0;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null)
        {
            throw new MissingComponentException("No rigidbody2D found");
        }
    }
    
    private void Update()
    {
        if (_counter < maxAmount) return;
        direction = direction == 1 ? -1 : 1;
        _counter = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        switch (horizontalMovement)
        {
            case true:
                _rigidbody2D.transform.position =
                    new Vector3(transform.position.x + (speed * direction * Time.deltaTime), transform.position.y, transform.position.z);
                break;
            
            case false:
                _rigidbody2D.transform.position = 
                    new Vector3(transform.position.x, transform.position.y + (speed * direction * Time.deltaTime), transform.position.z);
                break;
        }
        
        _counter += speed * Time.deltaTime;
    }
}
