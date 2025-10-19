using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using SystemRandom = System.Random;

public class playercontroller : MonoBehaviour, playercontroller.InPlayerController
{
    [SerializeField] public playermovement data;
    [SerializeField] private Animator animator;
    [SerializeField] private CollisionChecker collisionChecker;
    [SerializeField] private WallSlide wallSlide;
    private playerHealth hp;

    private Rigidbody2D rigid;
    private CapsuleCollider2D caps;
    public InputData input;
    private Vector2 velocity;
    private bool defaultQuerystartinColliders;

    private Vector2 currentCheckpoint;

    #region Interface
    public Vector2 moveInput => input.move;
    public event Action<bool, float> onGroundChange;
    public event Action onJump;
    #endregion

    [Header("Crouch Settings")]
    [SerializeField] private Vector2 crouchColliderSize = new Vector2(1.5f, 0.5f);
    [SerializeField] private float crouchSpeed = 2.0f;

    private bool firstTime = true;
    private bool isFalling = false;
    private Vector3 previousPosition;
    private float highestPosition;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] private AudioClip[] footStepSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;

    private float footstepTimer = 0.1f;
    private float footstepInterval = 0.3f;
    public bool isRespawning = false;
    /*
    [Header("Player Flip")]
    [SerializeField] private Transform playerTransform;

    
    /*
    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 20f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private Vector2 slideColliderSize = new Vector2(2f, 0.5f);
    private bool isSliding;
    private float slideTime;
    */

    private Vector2 originalColliderSize;
    private bool isCrouching;
    private float timeElapsed;

    public bool inputEnabled = true;

    public bool onMovingPlatform = false;
    public float rayDistance = 1.0f;
    [SerializeField] private LayerMask movingPlatformLayer;
    private Vector2 raycastOriginOffset = new Vector2(1.5f, -1f);

    public void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        caps = GetComponent<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();
        collisionChecker = GetComponent<CollisionChecker>();
        wallSlide = GetComponent<WallSlide>();
        hp = GetComponent<playerHealth>();

        defaultQuerystartinColliders = Physics2D.queriesStartInColliders;

        originalColliderSize = caps.size;
        previousPosition = transform.position;
        onMovingPlatform = false;
        /*
        if(playerTransform == null)
        {
            playerTransform = transform;
        }
        */
    }


    private void Update()
    {
        CheckIfOnMovingPlatform();
        timeElapsed += Time.deltaTime;

        if (inputEnabled)
        {
            CaptureInput();
        }
        //IsJumping();

        //Fall Damage
        if(!isGround)
        {
            if(transform.position.y < previousPosition.y && firstTime)
            {
                firstTime = false;
                isFalling = true;
                highestPosition = transform.position.y;
            }
            previousPosition = transform.position;
        }

        if(isGround && isFalling)
        {
            if(highestPosition - transform.position.y > 8)
            {
                hp.PlayerTakeDamage(4);
            }
            isFalling = false;
            firstTime = true;
        }

        
    }

    private void CaptureInput()
    {
        input = new InputData
        {
            jumpDown = Input.GetKeyDown(KeyCode.Space),
            jumphold = Input.GetKey(KeyCode.Space),
            move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

        if(data.snapInput)
        {
            input.move.x = Mathf.Abs(input.move.x) < data.horizontalDeadZone ? 0 : Mathf.Sign(input.move.x);
            input.move.y = Mathf.Abs(input.move.y) < data.verticalDeadZone ? 0 : Mathf.Sign(input.move.y);
        }

        if(input.jumpDown)
        {
            jumpisReady = true;
            jumponTime = timeElapsed;
        }

        input.crouchDown = Input.GetKeyDown(KeyCode.LeftControl);
        input.crouchHold = Input.GetKey(KeyCode.LeftControl);
        //input.slideDown = Input.GetKeyDown(KeyCode.LeftShift);
    }

    #region Crouch Mechanic
    private void ProcessCrouch()
    {
        if(input.crouchHold)
        {
            if(!isCrouching)
            {
                isCrouching = true;
                caps.size = crouchColliderSize;

                animator.SetBool("isCrouching", true);
                //Debug.Log("Crouching Started");
            }
            velocity.x = input.move.x * crouchSpeed; //adjusts the speed
        }
        else if(isCrouching)
        {
            isCrouching = false;
            caps.size = originalColliderSize;

            animator.SetBool("isCrouching", false);
            //Debug.Log("Crouching Stopped");
        }
    }
    #endregion

    /*private void FlipController()
    {
        if(input.move.x > 0 && !facingRight)
        {
            Flip();
        }
        else if(input.move.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        playerTransform.Rotate(0, 180, 0);
    }

    /*
    #region Slide Mechanic
    private void ProcessSlide()
    {
        if(input.slideDown && isGround && !isSliding)
        {
            isSliding = true;
            slideTime = Time.time + slideDuration;
            caps.size = slideColliderSize;
            velocity.x = input.move.x * slideSpeed;

            animator.SetBool("Slide", true);
        }

        if(isSliding)
        {
            if(Time.time > slideTime || velocity.x == 0)
            {
                isSliding = false;
                caps.size = originalColliderSize;

                animator.SetBool("Slide", false);
            }
        }
    }
    #endregion
    */
    private void FixedUpdate()
    {
        if (isRespawning)
        {
            return;
        }

        DetectCollision();
        ProcessJump();
        ProcessMovement();
        ProcessGravity();
        
        ProcessCrouch();
        
        if(!wallSlide.isWallSliding)
        {
            ApplyMovement();
        }

        audioSource.mute = onMovingPlatform;
        PlayFootstepSound();


        //ResetWallJump();
        //IsWallJumping();
        //ProcessSlide();
    }

    #region Player Collisions
    private float timeleftGround = float.MinValue;
    public bool isGround;

    public void DetectCollision()
    {
        Physics2D.queriesStartInColliders = false;

        bool groundHit = Physics2D.CapsuleCast(caps.bounds.center, caps.size, caps.direction, 0, Vector2.down, data.grounderDistance, ~data.playerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(caps.bounds.center, caps.size, caps.direction, 0, Vector2.up, data.grounderDistance, ~data.playerLayer);
        
        if(ceilingHit)
        {
            velocity.y = Mathf.Min(0, velocity.y);
        }

        if(!isGround && groundHit)
        {
            isGround = true;
            canCoyote = true;
            bufferedJump = true;
            jumpShort = false;
            //hasWallJumped = false;
            jumpCount = 0;
            onGroundChange?.Invoke(true,Mathf.Abs(velocity.y));

            PlayLandSound();
        }
        else if (isGround && !groundHit)
        {
            isGround = false;
            timeleftGround = timeElapsed;
            onGroundChange?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = defaultQuerystartinColliders;
    }
    #endregion

    #region Jump Funtion

    private bool jumpisReady;
    private bool bufferedJump;
    private bool jumpShort;
    private bool canCoyote;
    private float jumponTime;
    public bool isJumping;

    public int jumpCount = 0;
    [SerializeField] public int maxJumps = 2;
    [SerializeField] private float doubleJumpPower = 10f;

    private bool IsbufferedJump => bufferedJump && timeElapsed < jumponTime + data.jumpBufferTime;
    private bool CanCoyote => canCoyote && !isGround && timeElapsed < timeleftGround + data.coyoteTime;

    private void ProcessJump()
    {
        if (!jumpShort && !isGround && !input.jumphold && rigid.linearVelocity.y > 0)
        {
            jumpShort = true;
            //velocity.y *= 0.5f;
        }

        if(!jumpisReady && !IsbufferedJump)
        {
            return;
        }

      if ((isGround || CanCoyote || jumpCount < maxJumps) && jumpisReady)
        {
            ExcuteJump();
        }
        

        jumpisReady = false;

    }

    /*
    private void IsJumping()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            isJumping = true;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            isJumping = false;
        }
    }
    */

    private void ExcuteJump()
    {
        jumpShort = false;
        jumponTime = 0;
        bufferedJump = false;
        canCoyote = false;

        //jumpCount = 1;
        //velocity.y = data.jumpPower;
        
        if(isGround || jumpCount == 0)
        {
            jumpCount = 1;
            
            
        }
        else
        {
            jumpCount++;
            //velocity.y = data.doubleJumpPower;
        }
        
        velocity.y = Mathf.Clamp(velocity.y, -data.maxFallSpeed, data.maxJumpHeight);

        velocity.y = data.jumpPower;

        PlayJumpSound();
        onJump?.Invoke();
    }
    #endregion

    #region Horizontal
    
    private void ProcessMovement()
    { 
        //float cSpeed = isCrouch ? crouchSpeed : data.maxSpeed;
        if(input.move.x == 0)
        {
            var deceleration = isGround ? data.groundDeceleration : data.airDeceleration;
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            velocity.x = Mathf.MoveTowards(velocity.x, input.move.x * data.maxSpeed, data.acceleration * Time.fixedDeltaTime);
        }
    }
    #endregion

   

    #region Gravity

    private void ProcessGravity()
    {
        if(isGround && velocity.y <= 0f)
        {
            velocity.y = data.groundingForce;
        }
        else
        {
            var gravity = data.fallAcceleration;
            if(jumpShort && velocity.y > 0)
            {
                gravity *= data.jumpEndEarlyGravityModifier;

            }
            velocity.y = Mathf.MoveTowards(velocity.y, -data.maxFallSpeed, gravity * Time.fixedDeltaTime);
        }
    }
    #endregion

    public void TriggerPrayAnimation()
    {
        animator.SetTrigger("Pray");
    }

    public void SetCheckpoint(Vector2 checkpointPosition)
    {
        currentCheckpoint = checkpointPosition;

        playerHealth health = GetComponent<playerHealth>();
        if (health != null)
        {
            health.SetRespawnPosition(checkpointPosition);
        }
    }

    public void RespawnAtCheckpoint()
    {
        if (currentCheckpoint != Vector2.zero)
        {
            transform.position = currentCheckpoint;
        }
        //ResetState();
    }
    private void ApplyMovement() => rigid.linearVelocity = velocity;

    public struct InputData
    {
        public bool jumpDown;
        public bool jumphold;

        public bool crouchDown;
        public bool crouchHold;
        //public bool slideDown;

        public Vector2 move;
    }
    public interface InPlayerController
    {
        public event Action<bool, float> onGroundChange;

        public event Action onJump;
        public Vector2 moveInput { get; }
    }

    private void PlayFootstepSound()
    {
        if (isRespawning) return;

        // Log the status of onMovingPlatform flag
        //Debug.Log("onMovingPlatform: " + onMovingPlatform);

        if (onMovingPlatform)
        {
            //Debug.Log("Skipping footstep sound because player is on a moving platform.");
            
            return;  // Skip if on moving platform
        }

        if (isGround && input.move.x != 0 && !wallSlide.isWallSliding && timeElapsed - footstepTimer >= footstepInterval)
        {
            if (footStepSound.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, footStepSound.Length);
                AudioClip randomFootstep = footStepSound[randomIndex];
                audioSource.PlayOneShot(randomFootstep);
                footstepTimer = timeElapsed;
            }
        }

    }

    void CheckIfOnMovingPlatform()
    {
        Vector2 origin = (Vector2)transform.position + raycastOriginOffset;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayDistance, movingPlatformLayer);

        onMovingPlatform = hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = onMovingPlatform ? Color.green : Color.red;

        Vector2 origin = (Vector2)transform.position + raycastOriginOffset;
        Vector2 direction = Vector2.down * rayDistance;

        Gizmos.DrawLine(origin, origin + direction);
    }

    private void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    private void PlayLandSound()
    {
        audioSource.PlayOneShot(landSound);
    }
}
