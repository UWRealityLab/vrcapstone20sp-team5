using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    // public float thrust = 1.0f;
    public Rigidbody rb;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * speed;
    }
}
