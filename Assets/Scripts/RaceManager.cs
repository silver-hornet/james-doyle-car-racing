using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;

    public Checkpoint[] allCheckpoints;

    public int totalLaps;

    public CarController playerCar;
    public List<CarController> allAICars = new List<CarController>();
    public int playerPosition;
    public float timeBetweenPosCheck = 0.2f;
    float posCheckCounter;

    public float aiDefaultSpeed = 30f;
    public float playerDefaultSpeed = 30f;
    public float rubberBandSpeedMod = 3.5f;
    public float rubberBandAccel = 0.5f;

    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    float startCounter;
    public int countdownCurrent = 3;

    public int playerStartPosition;
    public int aiNumberToSpawn;
    public Transform[] startPoints;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        for (int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;

        UIManager.instance.countdownText.text = countdownCurrent.ToString();

        playerStartPosition = Random.Range(0, aiNumberToSpawn);
        playerCar.transform.position = startPoints[playerStartPosition].position;
        playerCar.theRB.transform.position = startPoints[playerStartPosition].position;
    }

    void Update()
    {
        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if (startCounter <= 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;
                UIManager.instance.countdownText.text = countdownCurrent.ToString();

                if (countdownCurrent == 0)
                {
                    isStarting = false;
                    UIManager.instance.countdownText.gameObject.SetActive(false);
                    UIManager.instance.goText.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            posCheckCounter -= Time.deltaTime;

            if (posCheckCounter <= 0)
            {
                playerPosition = 1;

                foreach (CarController aiCar in allAICars)
                {
                    if (aiCar.currentLap > playerCar.currentLap)
                    {
                        playerPosition++;
                    }
                    else if (aiCar.currentLap == playerCar.currentLap)
                    {
                        if (aiCar.nextCheckpoint > playerCar.nextCheckpoint)
                        {
                            playerPosition++;
                        }
                        else if (aiCar.nextCheckpoint == playerCar.nextCheckpoint)
                        {
                            if (Vector3.Distance(aiCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allCheckpoints[aiCar.nextCheckpoint].transform.position))
                            {
                                playerPosition++;
                            }
                        }
                    }
                }

                posCheckCounter = timeBetweenPosCheck;
                UIManager.instance.positionText.text = playerPosition + "/" + (allAICars.Count + 1);
            }

            // manager rubber banding
            if (playerPosition == 1)
            {
                foreach (CarController aiCar in allAICars)
                {
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed + rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
            }
            else
            {
                foreach (CarController aiCar in allAICars)
                {
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed - (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count + 1))), rubberBandAccel * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count + 1))), rubberBandAccel * Time.deltaTime);
            }
        }
    }
}
