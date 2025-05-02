using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitDoor : MonoBehaviour
{
    public bool isPlayerOverLapping = false;
    private void Update()
    {
        if (!LevelLogic.instance.IsEndTriggered)
        {
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.W)&&isPlayerOverLapping)
            {
                LevelLogic.instance.IsEndTriggered = false;
                SceneManager.LoadScene("Hub_World");
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
         
            isPlayerOverLapping = true;   
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
         
            isPlayerOverLapping = false;   
        }
    }
}
