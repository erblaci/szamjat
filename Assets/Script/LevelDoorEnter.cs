using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelDoorEnter : MonoBehaviour
{
    public string LevelToLoad;
    public bool isPlayerTouching = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerTouching = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerTouching = false;
        }
    }

    private void Update()
    {
        if (!isPlayerTouching)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            SceneManager.LoadScene(LevelToLoad);
        }
    }
}