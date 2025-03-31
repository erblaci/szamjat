//https://www.youtube.com/watch?v=RuvfOl8HhhM
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class enemy_movement : MonoBehaviour
{
    public GameObject Apont;
    public GameObject Bpont;
    private Rigidbody2D rb;
    private Transform currentpoint;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentpoint = Bpont.transform;
        
    }

    void Update()
    {
        Vector2 point = currentpoint.position - transform.position;
        if (currentpoint == Bpont.transform)
        {
            rb.linearVelocity = new Vector2(speed,0);
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed,0);
        }

        if (Vector2.Distance(transform.position, currentpoint.position)< 0.5f && currentpoint == Bpont.transform)
        {
            currentpoint = Apont.transform;
        }
        if (Vector2.Distance(transform.position, currentpoint.position)< 0.5f && currentpoint == Apont.transform)
        {
            currentpoint = Bpont.transform;
        }
    }
}
