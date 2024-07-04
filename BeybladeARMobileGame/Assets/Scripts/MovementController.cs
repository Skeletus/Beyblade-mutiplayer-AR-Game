using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxVelocityChange = 10f;
    [SerializeField] private float tiltAmount = 10f;

    private Rigidbody rigidBody;
    private Joystick joystick;
    private Vector3 velocityVector = Vector3.zero;

    private void Start()
    {
        joystick = GetComponentInChildren<Joystick>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // taking the joystick inputs
        float xMovementInput = joystick.Horizontal;
        float zMovementInput = joystick.Vertical;

        // calculate velocity vector
        Vector3 movementHorizontal = transform.right * xMovementInput;
        Vector3 movementVertical = transform.forward * zMovementInput;

        // calculate final movement velocity vector
        Vector3 movementVelocityVector = (movementHorizontal + movementVertical).normalized * speed;

        // apply movement 
        Move(movementVelocityVector);

        transform.rotation = Quaternion.Euler(joystick.Vertical * speed * tiltAmount,
            0, -joystick.Horizontal * speed * tiltAmount);
    }

    private void FixedUpdate()
    {
        if (velocityVector != Vector3.zero)
        {
            // get rigidbody's current velocity
            Vector3 velocity = rigidBody.velocity;
            Vector3 velocityChange = velocityVector - velocity;

            // apply force by the amount of velocity change to reach the target velocity
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            rigidBody.AddForce(velocityChange, ForceMode.Acceleration);
        }

    }

    private void Move(Vector3 movementVelocityVector)
    {
        velocityVector = movementVelocityVector;
    }

    public void EnableJoystick()
    {
        joystick.gameObject.SetActive(true);
    }

    public void DisableJoystick()
    {
        joystick.gameObject.SetActive(false);
    }
}
