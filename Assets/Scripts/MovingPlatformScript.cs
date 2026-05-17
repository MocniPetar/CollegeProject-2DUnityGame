using System;
using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    [SerializeField] private bool isMovingFromLeftToRight;
    [SerializeField] private float speed;
    private Rigidbody2D _rigidbody2D;

    [SerializeField] private float maxTopAmount;
    [SerializeField] private float maxBottomAmount;
    [SerializeField] private float maxLeftAmount;
    [SerializeField] private float maxRightAmount;
    [SerializeField] private int direction;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null)
        {
            throw new MissingComponentException("No rigidbody2D found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMovingFromLeftToRight)
        {
            if (_rigidbody2D.transform.position.y > maxTopAmount)
                direction = -1;
            
            if (_rigidbody2D.transform.position.y < maxBottomAmount)
                direction = 1;
            
            _rigidbody2D.transform.position = 
                new Vector3(transform.position.x, transform.position.y + (speed * direction * Time.deltaTime), transform.position.z);
        }
        
        if (isMovingFromLeftToRight)
        {
            if (_rigidbody2D.transform.position.y > maxLeftAmount)
                _rigidbody2D.transform.position =
                    new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);

            else if (_rigidbody2D.transform.position.y < maxRightAmount)
                _rigidbody2D.transform.position =
                    new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }
}
