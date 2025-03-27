using System;
using System.Collections;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction = Vector2.right;
    private Vector2 movement;
    private float walkspeed = 2f;
    private int runstate = 1;
    bool isRunning = false;
    bool isChangingSpeed = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        MovePlayer();
    }

    private void GetInput()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(hor, 0f);
        if (hor<0)
        {
            direction = Vector2.left;
        }else if (hor > 0)
        {
            direction = Vector2.right;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
            runstate = 1;
        }

        if (isRunning&&rb.linearVelocity.x >0&&!isChangingSpeed)
        {
            StartCoroutine(ChangeRunstate());
        }
    }

    public IEnumerator ChangeRunstate()
    {
        isChangingSpeed = true;
        yield return new WaitForSeconds(2f);
        if (isRunning&&rb.linearVelocity.x>1&&runstate<4)
        {
            Debug.Log(runstate);
            runstate++;
        }
        isChangingSpeed = false;
    }
    
    private void MovePlayer()
    {
        if (!isRunning)
        {
            rb.AddForce(movement*walkspeed,ForceMode2D.Force);
        }else if (isRunning)
        {
            rb.AddForce(movement*walkspeed*(runstate+1),ForceMode2D.Force);
        }
        
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, direction*1f, Color.red);
    }
}
