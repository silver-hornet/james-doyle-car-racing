using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;
    public float maxSpeed;
    public float forwardAccel = 8f;
    public float reverseAccel = 4f;
    float speedInput;

    void Start()
    {
        theRB.transform.parent = null;
    }

    void Update()
    {
        speedInput = 0;

        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }

        transform.position = theRB.position;
    }

    void FixedUpdate()
    {
        theRB.AddForce(new Vector3(0f, 0f, speedInput));
    }
}
