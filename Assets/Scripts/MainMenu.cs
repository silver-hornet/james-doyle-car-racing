using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public GameObject raceSetupPanel;
    public GameObject trackSelectPanel;
    public GameObject racerSelectPanel;
    public Image trackSelectImage;
    public Image racerSelectImage;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (RaceInfoManager.instance.enteredRace)
        {
            trackSelectImage.sprite = RaceInfoManager.instance.trackSprite;
            racerSelectImage.sprite = RaceInfoManager.instance.racerSprite;

            OpenRaceSetup();
        }

        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToLoad + "_unlocked", 1);
    }

    public void StartGame()
    {
        RaceInfoManager.instance.enteredRace = true;
        SceneManager.LoadScene(RaceInfoManager.instance.trackToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void OpenRaceSetup()
    {
        raceSetupPanel.SetActive(true);
    }

    public void CloseRaceSetup()
    {
        raceSetupPanel.SetActive(false);
    }

    public void OpenTrackSelect()
    {
        trackSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void CloseTrackSelect()
    {
        trackSelectPanel.SetActive(false);
        OpenRaceSetup();
    }

    public void OpenRacerSelect()
    {
        racerSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void CloseRacerSelect()
    {
        racerSelectPanel.SetActive(false);
        OpenRaceSetup();
    }
}
