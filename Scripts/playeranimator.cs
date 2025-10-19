using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playeranimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    
    

    [Header("Settings")]
    [SerializeField, Range(1f, 3f)] private float maxIdleSpeed = 2;
    [SerializeField] private float maxTilt = 5;
    [SerializeField] private float tiltSpeed = 20;

    private playercontroller.InPlayerController player;
    private bool isGround;
    private bool isInCutscene = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        player = GetComponentInParent<playercontroller.InPlayerController>();
        
    }

    private void OnEnable()
    {
        player.onJump += OnJump;
        player.onGroundChange += OnGroundChange;

    }

    private void OnDisable()
    {
        player.onJump -= OnJump;
        player.onGroundChange -= OnGroundChange;
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        if (isInCutscene)
        {
            return;
        }

        HandleSpriteFlip();
        HandleIdleSpeed();
        HandleCharacterTilt();
        HandleMovementAnimation();
        HandleSlideAnimation();
        
    }

    private void HandleSpriteFlip()
    {
        
            if (player.moveInput.x != 0)
            {
                sprite.flipX = player.moveInput.x < 0;
            }
        
    }

    public void HandleIdleSpeed()
    {
        var inputStrength = Mathf.Abs(player.moveInput.x);
        animator.SetFloat(idleSpeedKey, Mathf.Lerp(1, maxIdleSpeed, inputStrength));
    }

    private void HandleCharacterTilt()
    {
        var runningTilt = isGround ? Quaternion.Euler(0, 0, maxTilt * player.moveInput.x) : Quaternion.identity;
        animator.transform.up = Vector3.RotateTowards(animator.transform.up, runningTilt * Vector2.up, tiltSpeed * Time.deltaTime, 0f);
    }

    private void HandleMovementAnimation()
    {
        var speed = Mathf.Abs(player.moveInput.x);
        bool isRunning = speed > 0.1f;

        animator.SetBool(runningKey, isRunning);

    }  

    public void HandleSlideAnimation()
    {
        bool isSliding = player.moveInput.x != 0 && Input.GetKey(KeyCode.LeftShift);
        animator.SetBool(slideKey, isSliding);

    }

    private void OnJump()
    {
        animator.SetTrigger(jumpKey);
        animator.ResetTrigger(groundKey);
    }

    private void OnGroundChange(bool ground, float impact)
    {
        isGround = ground;

        if(ground)
        {
            animator.SetTrigger(groundKey);
            
        }
    }

    private static readonly int groundKey = Animator.StringToHash("Ground");
    private static readonly int idleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int jumpKey = Animator.StringToHash("Jump");
    private static readonly int runningKey = Animator.StringToHash("isRunning");
    //private static readonly int crouchKey = Animator.StringToHash("isCrouching");
    private static readonly int slideKey = Animator.StringToHash("isSliding");


    public void SetCutsceneMode(bool active)
    {
        isInCutscene = active;

        if (active)
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("Idle");
        }

    }

}
