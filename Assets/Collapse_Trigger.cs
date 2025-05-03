using System;
using Unity.VisualScripting;
using UnityEngine;

public class Collapse_Trigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelLogic.instance.IsEndTriggered = true;
            LevelLogic.instance.IsRunning = true;
            Destroy(gameObject);
        }
        
    }
}
