using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    
    //keybinds
    
    KeyCode LeftKey = KeyCode.A;
    KeyCode RightKey = KeyCode.D;
    KeyCode JumpKey = KeyCode.Space;
    KeyCode RunKey = KeyCode.LeftShift;
    
    
    
    
   [SerializeField] private ParticleSystem dust_particle;
   [SerializeField] private BoxCollider2D grab_hitbox;
    [SerializeField] private SpriteRenderer character_sprite;
   public float wallRunSpeed = 20f;
   public float wallJumpForce = 8f;
   public float wallDetectionDistance = 2f;
   public LayerMask wallLayer;
   private float preserved_velocity = 0;

   private float coyote_time_max = 0.18f;
   private float coyote_time = 0.18f;

   //PauseMenu
   [SerializeField] private GameObject PauseMenu;
   private bool isGamePaused = false;
   
   private bool isWpressed = false;
   private bool IsSuperJumping=false;
   private bool canDash = true;
   private bool isTouchingWall;
   private Vector3 wallNormal;
   private bool inFastFall=false;
   private Vector2 WallJumpDirection=Vector2.right;
    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
        isGamePaused = false;
        PauseMenu.SetActive(false);
    }
    private void CheckForRebind()
    {
        
    }
    private void Update()
    {
            
        GetInput();
        MovePlayer();
        Dust();
        CheckForWall();
        HandleWallRun();
        CheckForFalling();
        
    }

    
    void CheckForWall() //csekkolja ,hogy vannak falak,amikre lehet felfutni
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

    public void CheckForFalling() //ha sokat vagy a levegőben akkor gyorsabban esel le
    {
        if (!isOnGround()&&!isWallClimbing&&!inFastFall)
        {
            inFastFall = true;
            StartCoroutine(FastFalling());
        }
    }
    public IEnumerator FastFalling()
    {
        yield return new WaitForSeconds(0.5f);
        if (!isOnGround()&&!isWallClimbing)
        {
            Debug.Log("fastfalling");
            rb.AddForce(Vector2.down*3f, ForceMode2D.Impulse);
        }
       inFastFall = false;
    }

    public IEnumerator WallClimbing_Check() //csekkolja,hogy még mindig futsz-e a falon és ha nem akkor vissza rak a rendes futás módba
    {
       // rb.linearVelocity = new Vector2(preserved_velocity, wallRunSpeed);
        yield return new WaitForSeconds(0.2f);
        rb.linearVelocity = new Vector2(preserved_velocity, 0);
        isWallClimbing = false;
    }
    
    
    private void Dust()//Ha elég gyorsan fut a játékos porzik a nyoma
    {
        if (runstate>1&&!dust_particle.isPlaying&&isOnGround())
        {
          //  Debug.Log("Dust");
            dust_particle.Play();
        }
        else if(runstate==1||dust_particle.isPlaying||!isOnGround())
        {
           dust_particle.Stop();
        }
    }
    private bool isOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up*-1f, 1.4f,LayerMask.GetMask("Ground"));
       
        if (hit.collider != null)
        {
           
            return true;
        }
        return false;
    }
    private bool isNextToWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position-transform.up*-0.5f, direction, 1.4f,LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            wallNormal = hit.normal;
            return true;
        }
        return false;
    }

    public IEnumerator ChangeDirectionCD()//ha irányt váltasz futás közben megállit és megforditja az irányod.
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGamePaused=!isGamePaused;
            if (isGamePaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
            PauseMenu.SetActive(isGamePaused);
            
        }
        isWpressed = Input.GetKey(KeyCode.W);
        if (isOnGround())
        {
            coyote_time = coyote_time_max;
        }
        else
        {
            coyote_time-=Time.deltaTime;
        }
        float hor = Input.GetAxisRaw("Horizontal");
        movement = new Vector2(hor, 0f);
        if (!isWallClimbing)
        {
            if (hor<0)
            {
                if (direction == Vector2.right)
                {
                    character_sprite.flipX = true;
                    direction = Vector2.left;
                    StartCoroutine(ChangeDirectionCD());
                }
            
            
            }else if (hor > 0)
            {
                if (direction == Vector2.left)
                {
                    character_sprite.flipX = false;
                    direction = Vector2.right;
                    StartCoroutine(ChangeDirectionCD());
                }
            
           
            }
        }
        else
        {
            if (hor<0)
            {
                if (WallJumpDirection == Vector2.right)
                {
                    WallJumpDirection = Vector2.left;
                    
                }
            
            
            }else if (hor > 0)
            {
                if (WallJumpDirection == Vector2.left)
                {
                    WallJumpDirection = Vector2.right;
                    
                }
            
           
            }
        }

        if(Input.GetKeyDown(KeyCode.X)&&canDash)
        {
            StartCoroutine("Dash");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log(canJump);
            if (!isWallClimbing&&coyote_time>0)
            {
                canJump = false;
                rb.AddForce(new Vector2(0f, 4f), ForceMode2D.Impulse);
                canJump = true;
            }
            else if(isWallClimbing&&WallJumpDirection!=direction)
            {
               
                Vector2 jumpDirection =new Vector2(wallNormal.x,wallNormal.y) + Vector2.up;
                rb.linearVelocity = new Vector2(jumpDirection.x * wallJumpForce, wallJumpForce);
                isWallClimbing = false;
                direction = -direction;
                if (direction==Vector2.right)
                {
                    character_sprite.flipX = false;
                }else if (direction == Vector2.left)
                {
                    character_sprite.flipX = true;
                }
            }
            
        }

        if (!isOnGround()&&Input.GetKeyDown(KeyCode.S))
        {
            if (!(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D)))
            {
                GroundPound();
            }
            else
            {
                Dive();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.W)&&runstate>1&&!IsSuperJumping)
        {
            SuperJump();
        }
        if (Input.GetKey(KeyCode.W) && IsSuperJumping)
        {
            

            // Allow slow horizontal movement
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontalInput * 1.5f, rb.linearVelocity.y);
        }
        if (Input.GetKeyUp(KeyCode.W) && IsSuperJumping)
        {
            ReleaseJump();
        }
        if (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift))
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

        if (isOnGround())
        {
            canDash = true;
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

    private void ReleaseJump()
    {
        float finalForce = 30; 
       
        rb.velocity = new Vector2(rb.velocity.x, 0); 
        rb.AddForce(Vector2.up * finalForce, ForceMode2D.Impulse);
        IsSuperJumping = false;
    }

    private void SuperJump()
    {
        IsSuperJumping = true;
        
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Freeze horizontal movement
        
    }

    public IEnumerator Dash()
    {
        canDash=false;
        rb.AddForce(direction*6,ForceMode2D.Impulse);
        yield return new WaitForSeconds(1.5f);
        
    }

    public void GroundPound()
    {
       rb.linearVelocityX = 0;
      rb.AddForce(Vector2.down*20, ForceMode2D.Impulse);
      rb.linearVelocityX = 0;
    }

    public void Dive()
    {
        rb.AddForce(new Vector2(direction.x*10,-10), ForceMode2D.Impulse);
    }
    public IEnumerator ChangeRunstate()//ha eleget futsz lassitás nélkül,akkor felgyorsulsz,elenkező esetben lelassulsz.
    {
        isChangingSpeed = true;
        yield return new WaitForSeconds(1f);
        if (isRunning&&rb.linearVelocity.x>0.5f&&runstate<4&&isOnGround())
        {
            Debug.Log(runstate);
            runstate++;
        }

        if (rb.linearVelocity.x<0.5f&&isChangingSpeed&&!isWallClimbing)
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
            SceneManager.LoadScene("Hub_World"); //Akkor a mostanitól eggyel kövi scenere ugrás
        }
    }
   
}
