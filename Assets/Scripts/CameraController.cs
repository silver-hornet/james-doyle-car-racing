using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CarController target;
    Vector3 offsetDir;

    public float minDistance = 20f;
    public float maxDistance = 50f;
    float activeDistance;

    void Start()
    {
        offsetDir = transform.position - target.transform.position;
        activeDistance = minDistance;
        offsetDir.Normalize();
    }

    void Update()
    {
        activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.velocity.magnitude / target.maxSpeed));
        transform.position = target.transform.position + (offsetDir * activeDistance);
    }
}
