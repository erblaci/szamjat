using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelLogic : MonoBehaviour
{
    public static LevelLogic instance;
    public GameObject baby_prefab;
    public float maxTime = 300;
    public float currentTime;
    public TMPro.TextMeshProUGUI timerGUI;
    public bool IsRunning = false;
    public bool IsEndTriggered = false;
    public int BabiesFound = 0;
    public Slider TimerBar;

    public void Awake()
    {
        currentTime=maxTime;
        if (instance==null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (!IsRunning)
        {
            TimerBar.gameObject.SetActive(false);
            
            return;
        }
        TimerBar.gameObject.SetActive(true);
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
        TimerBar.value = 1/maxTime*currentTime;
                
        
    }

    public void AddBaby()
    {
        BabiesFound++;
        GameObject player = GameObject.FindWithTag("Player");
       GameObject newBaby = Instantiate(baby_prefab,player.transform.position+new Vector3(0,0.5f*BabiesFound,0), Quaternion.identity);
       SpriteRenderer babySprite = newBaby.GetComponent<SpriteRenderer>();
       babySprite.sortingOrder = 40 + BabiesFound;
       newBaby.transform.parent = player.transform;
       
    }
    
}
