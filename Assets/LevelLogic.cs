using System;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    public float maxTime = 300;
    public float currentTime;
    public TMPro.TextMeshProUGUI timerGUI;
    public bool IsRunning = false;
    public bool IsEndTriggered = false;
    public int BabiesFound = 0;

    public void Awake()
    {
        currentTime=maxTime;
        
    }

    private void Update()
    {
        if (!IsRunning)
        {
            return;
        }
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Max(currentTime, 0f);
        UpdateText();
        if (currentTime==0)
        {
            IsRunning = false;
        }
    }

    public void UpdateText()
    {
       
        currentTime = Mathf.Max(currentTime, 0f);
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerGUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                
        
    }
    
}
