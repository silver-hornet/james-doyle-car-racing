﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectButton : MonoBehaviour
{
    public string trackSceneName;
    public Image trackImage;
    public int raceLaps = 4;

    public GameObject unlockedText;
    bool isLocked;
    public string trackToUnlockOnWin;

    void Start()
    {
        if (!PlayerPrefs.HasKey(trackSceneName + "_unlocked"))
        {
            isLocked = true;
            trackImage.color = Color.gray;
            unlockedText.SetActive(true);
        }
    }
    public void SelectTrack()
    {
        if (!isLocked)
        {
            RaceInfoManager.instance.trackToLoad = trackSceneName;
            RaceInfoManager.instance.noOfLaps = raceLaps;
            RaceInfoManager.instance.trackSprite = trackImage.sprite;

            MainMenu.instance.trackSelectImage.sprite = trackImage.sprite;
            MainMenu.instance.CloseTrackSelect();

            RaceInfoManager.instance.trackToUnlock = trackToUnlockOnWin;
        }
    }
}
