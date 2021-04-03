using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TMP_Text lapCounterText;
    public TMP_Text bestLapTimeText;
    public TMP_Text currentLapTimeText;
    public TMP_Text positionText;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
