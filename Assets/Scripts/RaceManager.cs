﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public List<CarController> carsToSpawn = new List<CarController>();

    public bool raceCompleted;

    public string raceCompleteScene;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        totalLaps = RaceInfoManager.instance.noOfLaps;
        aiNumberToSpawn = RaceInfoManager.instance.noOfAI;

        for (int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNumber = i;
        }

        isStarting = true;
        startCounter = timeBetweenStartCount;

        UIManager.instance.countdownText.text = countdownCurrent.ToString();

        playerStartPosition = Random.Range(0, aiNumberToSpawn + 1);
        playerCar = Instantiate(RaceInfoManager.instance.racerToUse, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation);
        playerCar.isAI = false;
        playerCar.GetComponent<AudioListener>().enabled = true;

        CameraSwitcher.instance.SetTarget(playerCar);

        //playerCar.transform.position = startPoints[playerStartPosition].position;
        //playerCar.theRB.transform.position = startPoints[playerStartPosition].position;

        for (int i = 0; i < aiNumberToSpawn + 1; i++)
        {
            if (i != playerStartPosition)
            {
                int selectedCar = Random.Range(0, carsToSpawn.Count);
                allAICars.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation));

                if (carsToSpawn.Count > aiNumberToSpawn - i)
                {
                    carsToSpawn.RemoveAt(selectedCar);
                }
            }
        }

        UIManager.instance.positionText.text = (playerStartPosition + 1) + "/" + (allAICars.Count + 1);
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

    public void FinishRace()
    {
        raceCompleted = true;

        switch (playerPosition)
        {
            case 1:
                UIManager.instance.raceResultText.text = "You finished 1st";

                if (RaceInfoManager.instance.trackToUnlock != "")
                {
                    if (!PlayerPrefs.HasKey(RaceInfoManager.instance.trackToUnlock + "_unlocked"))
                    {
                        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToUnlock + "_unlocked", 1);
                        UIManager.instance.trackUnlockedMessage.SetActive(true);
                    }
                }

                break;
            case 2:
                UIManager.instance.raceResultText.text = "You finished 2nd";
                break;
            case 3:
                UIManager.instance.raceResultText.text = "You finished 3rd";
                break;
            default:
                UIManager.instance.raceResultText.text = "You finished " + playerPosition + "th";
                break;
        }

        UIManager.instance.resultsScreen.SetActive(true);
    }

    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompleteScene);
    }
}
