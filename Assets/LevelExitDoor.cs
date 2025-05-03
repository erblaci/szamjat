using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class LevelExitDoor : MonoBehaviour
{
    public bool isPlayerOverLapping = false;
    public GameObject RatingPage;
    public Image Rating_IMG;
    public Sprite[] ratings;
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
                StartCoroutine(ShowScore());

            }
        }
    }

    public IEnumerator<WaitForSeconds> ShowScore()
    {
        RatingPage.gameObject.SetActive(true);
        if (LevelLogic.instance.LevelScore >= 4000)
        {
            Rating_IMG.sprite = ratings[0];
        }
        else
        {
            Rating_IMG.sprite = ratings[1];
        }
        yield return new WaitForSeconds(3f);
        RatingPage.gameObject.SetActive(false);
        SceneManager.LoadScene("Hub_World");
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
