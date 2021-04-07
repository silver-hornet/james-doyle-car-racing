﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher instance;

    public GameObject[] cameras;
    int currentCam;

    public CameraController topDownCam;
    public Cinemachine.CinemachineVirtualCamera cineCam;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCam++;

            if (currentCam >= cameras.Length)
            {
                currentCam = 0;
            }

            for (int i = 0; i < cameras.Length; i++)
            {
                if (i == currentCam)
                {
                    cameras[i].SetActive(true);
                }
                else
                {
                    cameras[i].SetActive(false);
                }
            }
        }
    }

    public void SetTarget(CarController playerCar)
    {
        topDownCam.target = playerCar;
        cineCam.m_Follow = playerCar.transform;
        cineCam.m_LookAt = playerCar.transform;
    }
}
