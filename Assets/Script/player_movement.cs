using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    
    //Debug
   public TMPro.TextMeshProUGUI playerStateDebug;
    //keybinds
    
    KeyCode LeftKey = KeyCode.LeftArrow;
    KeyCode RightKey = KeyCode.RightArrow;
    KeyCode JumpKey = KeyCode.Space;
    KeyCode RunKey = KeyCode.LeftShift;

    public PlayerState playerState;
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Mach1,
        Mach2,
        Mach3,
        WallRunning,
        inAir,
        GroundPounding,
        SuperJumpStart,
        SuperJumpEnd,
    }
    
    
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
   
   private bool isGroundPounding = false;
   private bool isWpressed = false;
   private bool IsSuperJumping=false;
   private bool canDash = true;
   private bool isTouchingWall;
   private bool canCancelSuperJump = true;
   private Vector3 wallNormal;
   private bool inFastFall=false;
   private Vector2 WallJumpDirection=Vector2.right;
   bool isCheckingSpeed=false;
   
   
    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void Pause()
    {
        Time.timeScale = 0;
    }

    public IEnumerator SpeedControl()
    {
        if (!isCheckingSpeed)
        {


            isCheckingSpeed = true;
            if (isOnGround())
            {
                if (!isWallClimbing)
                {
                    if (Math.Abs(rb.linearVelocity.x) > runstate * walkspeed && runstate < 4)
                    {
                        runstate++;
                    }
                    else
                    {
                        yield return new WaitForSeconds(2);
                        if (Math.Abs(rb.linearVelocity.x) > runstate * walkspeed && runstate < 4)
                        {
                            runstate++;
                        }
                        else
                        {
                            runstate--;
                        }
                    }
                }
            }

            isCheckingSpeed = false;
        }
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

    public void ChangePlayerState()
    {
        if (isOnGround())
        {
            isGroundPounding = false;
        }
        if (playerState!= PlayerState.SuperJumpEnd&&!isWallClimbing&&!isOnGround()&&playerState!= PlayerState.SuperJumpStart&&!isGroundPounding)
        {
            playerState = PlayerState.inAir;
        }else
        if (rb.linearVelocity==Vector2.zero)
        {
            playerState = PlayerState.Idle;
        }else

        if (Math.Abs(rb.linearVelocityX)>0&&Math.Abs(rb.linearVelocityX)<=walkspeed&&!isRunning)
        {
            playerState = PlayerState.Walking;
        }else

        if (Math.Abs(rb.linearVelocityX)>walkspeed&&isRunning)
        {
            playerState = PlayerState.Running;
        }else if (IsSuperJumping)
        {
            playerState = PlayerState.SuperJumpStart;
        }else if (canCancelSuperJump&&!isOnGround()&&!isWallClimbing)
        {
            playerState = PlayerState.SuperJumpEnd;
        }else if (isWallClimbing)
        {
            playerState = PlayerState.WallRunning;
        }

        if (isGroundPounding)
        {
            playerState = PlayerState.GroundPounding;
        }
//        playerStateDebug.text = playerState.ToString();
    }
    private void Update()
    {
        StartCoroutine(SpeedControl());
        ChangePlayerState();
        SpeedDebug();
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
        if (isNextToWall() && isRunning&&!isGroundPounding)
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
            
            rb.AddForce(Vector2.down*3f, ForceMode2D.Impulse);
        }
       inFastFall = false;
    }

    public void SpeedDebug()
    {
        if (runstate==1)
        {
            character_sprite.color = Color.white;
        }
        if (runstate==2)
        {
            character_sprite.color = Color.green;
        }
        if (runstate==3)
        {
            character_sprite.color = Color.yellow;
        }
        if (runstate==4)
        {
            character_sprite.color = Color.red;
        }
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
            if (playerState != PlayerState.WallRunning&&coyote_time>0)
            {
                canJump = false;
                rb.AddForce(new Vector2(0f, 4f), ForceMode2D.Impulse);
                canJump = true;
            }
            else if(playerState == PlayerState.WallRunning&&WallJumpDirection!=direction)
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

        if (playerState == PlayerState.inAir&&Input.GetKeyDown(KeyCode.S))
        {
            if (!(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D))&&FarGroundCheck())
            {
                isGroundPounding=true;
                GroundPound();
            }
            else if(FarGroundCheck())
            {
                Dive();
            }
            
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
        

        if (playerState != PlayerState.inAir)
        {
            canDash = true;
        }

        if (IsSuperJumping)
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))&&canCancelSuperJump)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
                    IsSuperJumping=false;
                    rb.AddForce(new Vector2(Math.Abs(preserved_velocity)*-1, 0f), ForceMode2D.Impulse);
                    
                }
                if (Input.GetKey(KeyCode.D))
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
                    IsSuperJumping=false;
                    rb.AddForce(new Vector2(Math.Abs(preserved_velocity), 0f), ForceMode2D.Impulse);
                    
                }
            }
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 1.4f,LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                IsSuperJumping = false;
            }
            
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

    public IEnumerator SuperJumpCancelCD()
    {
        canDash=false;
        yield return new WaitForSeconds(0.5f);
        canCancelSuperJump = true;
    }
    private void ReleaseJump()
    {
        float finalForce = 30; 
       StartCoroutine(SuperJumpCancelCD());
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); 
        rb.AddForce(Vector2.up * finalForce, ForceMode2D.Impulse);
       
    }

    private bool FarGroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up*-1f, 4f,LayerMask.GetMask("Ground"));
       
        if (hit.collider != null)
        {
           
            return false;
        }
        return true; 
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
      runstate = 1;
    }

    public void Dive()
    {
        rb.AddForce(new Vector2(direction.x*10,-10), ForceMode2D.Impulse);
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
