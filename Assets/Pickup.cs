using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    private void Update()
    {
        float y=Mathf.Sin(Time.time)/500+transform.position.y;
        transform.position = new Vector3(transform.position.x,y,transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        LevelLogic.instance.LevelScore += 100;
        Destroy(gameObject);
    }
}
