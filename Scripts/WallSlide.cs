using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlide : MonoBehaviour
{

    private CollisionChecker collisionChecker;
    private playercontroller playerController;
    [SerializeField] private Animator animator;
    [SerializeField] public playermovement data;
    private Rigidbody2D rigid;

    private bool canWallSlide;
    public bool isWallSliding;
    private bool isWallJumping;
    private bool canWallJump;
    private bool hasWallJumped;

    [SerializeField] private float wallJumpForceX = 5f;
    [SerializeField] private float wallJumpForceY = 10f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1, 1); // Direction for wall jumping
    [SerializeField] private float wallSlideSpeed = 2f;

    [SerializeField] private float wallJumpCooldown = 0.1f; // Cooldown time between wall jumps
    private bool wallJumpCooldownActive = false;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        collisionChecker = GetComponent<CollisionChecker>();
        playerController = GetComponent<playercontroller>();
        rigid = GetComponent<Rigidbody2D>();

        wallJumpDirection.Normalize();
    }
    private void Update()
    {
        cWallSlide();
        WallSlideCheck();

        if (isWallSliding && playerController.inputEnabled)
        {
            if (playerController.input.jumpDown && !wallJumpCooldownActive) // Check inputEnabled state
            {
                WallJump();
            }
        }
    }

    private void FixedUpdate()
    {
        HandleWallSlide();
    }

    private void HandleWallSlide()
    {
        if (isWallJumping) return;

        if ((collisionChecker.isWallDetectedLeft || collisionChecker.isWallDetectedRight) && canWallSlide)
        {
                isWallSliding = true;
                animator.SetBool("isWallSliding", isWallSliding);


            float smoothVelocityY = Mathf.Max(rigid.linearVelocity.y, -wallSlideSpeed);
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, smoothVelocityY);

            // Prevent upward motion when holding jump
            if (playerController.input.jumphold)
            {
                rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, Mathf.Clamp(rigid.linearVelocity.y, float.MinValue, 0)); // Keep moving down
            }

        }
        else
        {
            isWallSliding = false;
            animator.SetBool("isWallSliding", false);
        }
    }

    private void cWallSlide()
    {
        if((collisionChecker.isWallDetectedLeft || collisionChecker.isWallDetectedRight) && !collisionChecker.isGrounded)
        {

            if ((playerController.moveInput.x < 0 && collisionChecker.isWallDetectedLeft) ||
                (playerController.moveInput.x > 0 && collisionChecker.isWallDetectedRight))
            {
                canWallSlide = true;
                canWallJump = true;
            }
            else
            {
                canWallSlide = false; // Disengage if no input and coyote time expired
                canWallJump = false;
            }
        }
        else
        {
            canWallSlide = false;
            canWallJump = false;
        }

    }
    private void WallJump()
    {
        if(!isWallSliding)
        {
            return;
        }

        float jumpDirection = collisionChecker.isWallDetectedLeft ? 1 : -1;

        // Use AddForce for a smoother jump
        Vector2 jumpForce = new Vector2(wallJumpForceX * 0.7f * jumpDirection, wallJumpForceY);
        rigid.AddForce(jumpForce, ForceMode2D.Impulse);

        /*
        Vector2 jumpForce = new Vector2(wallJumpForceX * jumpDirection, wallJumpForceY * 0.5f);
        rigid.velocity = jumpForce;
        */

        hasWallJumped = true;
        isWallJumping = true;
        playerController.jumpCount = 0;

        canWallSlide = false;
        isWallSliding = false;
        canWallJump = false;

        wallJumpCooldownActive = true;
        StartCoroutine(ResetWallJumpCooldown());
        StartCoroutine(AllowDoubleJumpAfterWallJump(0.1f));
        StartCoroutine(ReenableWallSlide(0.3f));

        StartCoroutine(DisableInputTemporarily(0.01f));
    }

    private IEnumerator AllowDoubleJumpAfterWallJump(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerController.jumpCount = 1;
    }

    private IEnumerator ResetWallJumpCooldown()
    {
        yield return new WaitForSeconds(wallJumpCooldown); // Wait for the cooldown duration
        wallJumpCooldownActive = false; // Reset the cooldown
    }

    private IEnumerator DisableInputTemporarily(float duration)
    {
        playerController.inputEnabled = false; // Disable input in playercontroller
        yield return new WaitForSeconds(duration);
        playerController.inputEnabled = true; // Re-enable input after duration
    }

    private IEnumerator ReenableWallSlide(float delay)
    {
        yield return new WaitForSeconds(delay);
        isWallJumping = false; // Reset the jump state
        canWallSlide = true;
    }

    private void WallSlideCheck()
    {
        if(!collisionChecker.isGrounded && rigid.linearVelocity.y < 0)
        {
            canWallSlide = true;
        }
    }
}
