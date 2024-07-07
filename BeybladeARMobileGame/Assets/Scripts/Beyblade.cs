using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beyblade : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 3600f;
    [SerializeField] private bool doSpin = false;

    [SerializeField] private GameObject playerGraphics;
    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (doSpin)
        {
            playerGraphics.transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        }
    }

    public float GetSpinSpeed()
    {
        return spinSpeed;
    }

    public void SlowSpinSpeed(float amount)
    {
        spinSpeed -= amount;
    }
}
