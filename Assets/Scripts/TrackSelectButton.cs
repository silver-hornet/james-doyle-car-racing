using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectButton : MonoBehaviour
{
    public string trackSceneName;
    public Image trackImage;
    public int raceLaps = 4;

    public void SelectTrack()
    {
        RaceInfoManager.instance.trackToLoad = trackSceneName;
        RaceInfoManager.instance.noOfLaps = raceLaps;
        RaceInfoManager.instance.trackSprite = trackImage.sprite;

        MainMenu.instance.trackSelectImage.sprite = trackImage.sprite;
        MainMenu.instance.CloseTrackSelect();
    }
}
