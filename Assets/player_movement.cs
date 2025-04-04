using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction = Vector2.right;
    private Vector2 movement;
    private float walkspeed = 3f;
    private int runstate = 1;
    bool isRunning = false;
    bool isChangingSpeed = false;
    bool canJump = true;
    bool isWallClimbing = false;
    bool isChangingDirection = false;
   [SerializeField] private ParticleSystem dust_particle;
   [SerializeField] private BoxCollider2D grab_hitbox;
    
   public float wallRunSpeed = 20f;
   public float wallJumpForce = 8f;
   public float wallDetectionDistance = 2f;
   public LayerMask wallLayer;
   private float preserved_velocity = 0;
   
   private bool isTouchingWall;
   private Vector3 wallNormal;
    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
        GetInput();
        MovePlayer();
        Dust();
        CheckForWall();
        HandleWallRun();
        
    }

    void CheckForWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, wallDetectionDistance, wallLayer))
        {
            isTouchingWall = true;
            wallNormal = hit.normal;
        }
        else if (Physics.Raycast(transform.position, -transform.right, out hit, wallDetectionDistance, wallLayer))
        {
            isTouchingWall = true;
            wallNormal = hit.normal;
        }
        else
        {
            isTouchingWall = false;
        }
    }

    void HandleWallRun()
    {
        if (isNextToWall() && isRunning)
        {
            preserved_velocity = direction.x * walkspeed * ((runstate + 1) / 2);
            isWallClimbing = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallRunSpeed*((runstate+1)/2));
        }
        else
        {
            if (isWallClimbing)
            {
               StartCoroutine(WallClimbing_Check());
            }
          
        }
    }


    public IEnumerator WallClimbing_Check()
    {
       // rb.linearVelocity = new Vector2(preserved_velocity, wallRunSpeed);
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = new Vector2(preserved_velocity, 0);
        isWallClimbing = false;
    }
    
    
    private void Dust()//Ha elég gyorsan fut a játékos porzik a nyoma
    {
        if (runstate>1&&!dust_particle.isPlaying)
        {
          //  Debug.Log("Dust");
            dust_particle.Play();
        }
        else if(runstate==1&&dust_particle.isPlaying)
        {
           dust_particle.Stop();
        }
    }
    private bool isOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up*-1f, 0.6f,LayerMask.GetMask("Ground"));
       
        if (hit.collider != null)
        {
           
            return true;
        }
        return false;
    }
    private bool isNextToWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position-transform.up*-0.5f, direction, 0.6f,LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            wallNormal = hit.normal;
            return true;
        }
        return false;
    }

    public IEnumerator ChangeDirectionCD()
    {
        int runstate_preserved = runstate;
       
        isChangingDirection = true;
        if (rb.linearVelocityX!=null&&rb.linearVelocityX!=Mathf.Infinity&&rb.linearVelocityX!=-Mathf.Infinity)
        {
            rb.linearVelocityX=rb.linearVelocityX/(4-runstate_preserved);
        }
        
        yield return new WaitForSeconds(2f);
        
        isChangingDirection = false;
        runstate = runstate_preserved;
    }
    private void GetInput()
    {
        float hor = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(hor, 0f);
        if (hor<0)
        {
            if (direction == Vector2.right)
            {
                direction = Vector2.left;
                StartCoroutine(ChangeDirectionCD());
            }
            
            
        }else if (hor > 0)
        {
            if (direction == Vector2.left)
            {
                direction = Vector2.right;
                StartCoroutine(ChangeDirectionCD());
            }
            
           
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(canJump);
            if (!isWallClimbing&&canJump)
            {
                canJump = false;
                rb.AddForce(new Vector2(0f, 8f), ForceMode2D.Impulse);
                canJump = true;
            }
            else if(isWallClimbing)
            {
               
                Vector2 jumpDirection =new Vector2(wallNormal.x,wallNormal.y) + Vector2.up;
                rb.linearVelocity = new Vector2(jumpDirection.x * wallJumpForce, wallJumpForce);
                isWallClimbing = false;
                direction = -direction;
            }
            
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

        canJump = isOnGround();
        if (isRunning&&rb.linearVelocity.x >0&&!isChangingSpeed&&isOnGround())
        {
            StartCoroutine(ChangeRunstate());
        }

        

     /*   if (isNextToWall()&&!isOnGround()&&isRunning&&!isWallClimbing)
        {
            isWallClimbing = true;
            StartCoroutine(WallClimbing());
        }
        else
        {
            isWallClimbing = false;
            StopCoroutine(WallClimbing());
        }*/
    }

    public IEnumerator ChangeRunstate()
    {
        isChangingSpeed = true;
        yield return new WaitForSeconds(2f);
        if (isRunning&&rb.linearVelocity.x>0.2f&&runstate<4&&isOnGround())
        {
            Debug.Log(runstate);
            runstate++;
        }

        if (rb.linearVelocity.x<0.2f&&isChangingSpeed&&!isWallClimbing)
        {
            runstate--;
        }
        isChangingSpeed = false;
    }

    
    private void MovePlayer()
    {
        if (!isWallClimbing)
        {
            if (!isRunning)
            {
                rb.AddForce(movement*walkspeed,ForceMode2D.Force);
            }else if (isRunning)
            {
                rb.AddForce(direction*walkspeed*((runstate+1)/2),ForceMode2D.Force);
            }
        }
        
        
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position+transform.up*-0.5f, direction*1f, Color.red);
        Debug.DrawRay(transform.position, transform.up*-0.6f, Color.green);
    }
    private void OnTriggerEnter2D(Collider2D other) //https://www.youtube.com/watch?v=ztJPnBpae_0
    {
        if (other.tag == "Map_exit")  //ha a cube tagje Map_exit
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //Akkor a mostanitól eggyel kövi scenere ugrás
        }
    }
}
