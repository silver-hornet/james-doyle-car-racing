using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;
    public float maxSpeed;

    void Start()
    {
        theRB.transform.parent = null;
    }

    void Update()
    {
        theRB.AddForce(new Vector3(0f, 0f, 100f));
        transform.position = theRB.position;
    }
}
