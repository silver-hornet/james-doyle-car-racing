using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public Checkpoint[] allCheckpoints;

    void Start()
    {
        for (int i = 0; i < allCheckpoints.Length; i++)
        {
            allCheckpoints[i].cpNumber = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
