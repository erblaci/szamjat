using System;
using System.Collections;
using UnityEngine;

public class TP_Box : MonoBehaviour
{
    [SerializeField] private TP_Box ExitBox;
    public Transform exitPoint;          
    public float warpDelay = 0.3f;        // Optional delay before teleport
    public KeyCode warpKey = KeyCode.S;   // Default is down arrow/S

    private bool playerInPipe = false;
    private GameObject player;


    private void Awake()
    {
        player=GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (playerInPipe && Input.GetKeyDown(warpKey))
        {
            StartCoroutine(WarpPlayer());
        }
    }

   public IEnumerator WarpPlayer()
    {
        // Optional: play animation or sound here

        // Freeze player briefly
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        yield return new WaitForSeconds(warpDelay);

        // Move player to the exit point
        player.transform.position = ExitBox.exitPoint.position;

        rb.isKinematic = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPipe = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInPipe = false;
            player = null;
        }
    }
}
