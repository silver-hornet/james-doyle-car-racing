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
    public float turnStrength = 180f;
    float turnInput;

    bool grounded;
    public Transform groundRayPoint;
    public Transform groundRayPoint2;
    public LayerMask whatIsGround;
    public float groundRayLength = 0.75f;
    float dragOnGround;
    public float gravityMod = 10f;

    public Transform leftFrontWheel;
    public Transform rightFrontWheel;
    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f;
    public float emissionFadeSpeed = 50f;
    float emissionRate;

    public AudioSource engineSound;
    public AudioSource skidSound;
    public float skidFadeSpeed = 2f;

    int nextCheckpoint;
    public int currentLap = 1;
    public float lapTime;
    public float bestLapTime;

    public bool isAI;
    public int currentTarget;
    Vector3 targetPoint;
    public float aiAccelerateSpeed = 1f;
    public float aiTurnSpeed = 0.8f;
    public float aiReachPointRange = 5f;
    public float aiPointVariance = 3f;
    public float aiMaxTurn = 30f;
    float aiSpeedInput;
    float aiSpeedMod;

    void Start()
    {
        theRB.transform.parent = null;
        dragOnGround = theRB.drag;

        if (isAI)
        {
            targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
            RandomizeAITarget();
            aiSpeedMod = Random.Range(0.8f, 1.1f);
        }

        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
    }

    void Update()
    {
        lapTime += Time.deltaTime;

        if (!isAI)
        {
            var ts = System.TimeSpan.FromSeconds(lapTime);
            UIManager.instance.currentLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

            speedInput = 0;

            if (Input.GetAxis("Vertical") > 0)
            {
                speedInput = Input.GetAxis("Vertical") * forwardAccel;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                speedInput = Input.GetAxis("Vertical") * reverseAccel;
            }

            turnInput = Input.GetAxis("Horizontal");

            //if (grounded && Input.GetAxis("Vertical") != 0)
            //{
            //    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
            //}
        }
        else
        {
            targetPoint.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                SetNextAITarget();
            }

            Vector3 targetDirection = targetPoint - transform.position;
            float angle = Vector3.Angle(targetDirection, transform.forward);

            Vector3 localPos = transform.InverseTransformPoint(targetPoint);
            if (localPos.x < 0f)
            {
                angle = -angle;
            }

            turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

            if (Mathf.Abs(angle) < aiMaxTurn)
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
            }
            else
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
            }

            speedInput = aiSpeedInput * forwardAccel * aiSpeedMod;
        }

        // turning the wheels
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        //transform.position = theRB.position;

        // control particle emissions
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

        if (grounded && (Mathf.Abs(turnInput) > 0.5f || (theRB.velocity.magnitude < maxSpeed * 0.5f && theRB.velocity.magnitude != 0)))
        {
            emissionRate = maxEmission; 
        }

        if (theRB.velocity.magnitude <= 0.5f)
        {
            emissionRate = 0f;
        }

        for (int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;
            emissionModule.rateOverTime = emissionRate;
        }

        if (engineSound != null)
        {
            engineSound.pitch = 1f + ((theRB.velocity.magnitude / maxSpeed) * 2f);
        }

        if (skidSound != null)
        {
            if (Mathf.Abs(turnInput) > 0.5f)
            {
                skidSound.volume = 1f;
            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            normalTarget = hit.normal;
        }

        if (Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            normalTarget = (normalTarget + hit.normal) / 2f; // getting average of the two raycasts by dividing by 2
        }

            // when on ground, rotate to match the normal
            if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        if (grounded)
        {
            theRB.drag = dragOnGround;
            theRB.AddForce(transform.forward * speedInput * 1000f);
        }
        else
        {
            theRB.drag = 0.1f;
            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }

        if (theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }

        //Debug.Log(theRB.velocity.magnitude);

        transform.position = theRB.position;

        if (grounded && speedInput != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        }
    }

    public void CheckpointHit(int cpNumber)
    {
        if (cpNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == RaceManager.instance.allCheckpoints.Length)
            {
                nextCheckpoint = 0;
                LapCompleted();
            }
        }

        if (isAI)
        {
            if (cpNumber == currentTarget)
            {
                SetNextAITarget();
            }
        }
    }

    public void SetNextAITarget()
    {
        currentTarget++;

        if (currentTarget >= RaceManager.instance.allCheckpoints.Length)
        {
            currentTarget = 0;
        }

        targetPoint = RaceManager.instance.allCheckpoints[currentTarget].transform.position;
        RandomizeAITarget();
    }

    public void LapCompleted()
    {
        currentLap++;
        if (lapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = lapTime;
        }

        lapTime = 0f;

        if (!isAI)
        {
            var ts = System.TimeSpan.FromSeconds(bestLapTime);
            UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

            UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        }
    }

    public void RandomizeAITarget()
    {
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }
}
