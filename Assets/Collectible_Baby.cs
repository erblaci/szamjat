using System;
using UnityEngine;

public class Collectible_Baby : MonoBehaviour
{
    private float y;
   

    void Update()
    {
       Loop();
    }

    void Loop()
    {
        y=Mathf.Sin(Time.time)/200+transform.position.y;
        transform.position = new Vector3(transform.position.x,y,transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelLogic.instance.AddBaby();
            Destroy(gameObject);
        }
    }
}
