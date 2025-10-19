using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSlide : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 20f;
    [SerializeField] private float slideDuration = 0.8f;
    [SerializeField] private Vector2 slideColliderSize = new Vector2(1f, 0.2f);
    [SerializeField] private Vector2 slideColliderOffset = new Vector2(0f, -0.25f);
    [SerializeField] float rayLength = 0.05f;
    
    /*
    [SerializeField] private Vector2 slideBoxSize = new Vector2(1.5f, 0.3f);
    [SerializeField] private Vector2 slideBoxOffset = new Vector2(0, -0.3f);
    */
    //[SerializeField] private float slideRotationAngle = -90f;

    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    /*
    private Vector2 originalBoxSize;
    private Vector2 originalBoxOffset;
    */
    private Quaternion originalRotation;
    private CapsuleCollider2D caps;
    private BoxCollider2D box;
    private Rigidbody2D rigid;
    private bool isSliding;
    private float slideTime;

    private void Awake()
    {
        caps = GetComponent<CapsuleCollider2D>();
      
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        
        originalColliderSize = caps.size;
        originalColliderOffset = caps.offset;
        /*
        originalBoxOffset = box.offset;
        originalBoxSize = box.size;
        */
        //originalRotation = transform.rotation;
    }

    private void Update()
    {
        HandleSlide();
    }

    private void HandleSlide()
    {
        bool slideInput = Input.GetKeyDown(KeyCode.LeftShift);

        if(slideInput && !isSliding && IsGrounded())
        {
            isSliding = true;
            slideTime = Time.time + slideDuration;

            // rotates the player - 90 degrees
            //transform.rotation = Quaternion.Euler(0, 0, slideRotationAngle);
            
            caps.size = slideColliderSize;
            caps.offset = slideColliderOffset;
            //caps.direction = CapsuleDirection2D.Horizontal;

            /*
            box.size = slideBoxSize;
            box.offset = slideBoxOffset;
            */

            // applies the initial force to start the slide
            rigid.linearVelocity = new Vector2(slideSpeed * transform.localScale.x, rigid.linearVelocity.y);

            animator.SetBool("isSliding", true);
        }

        if(isSliding)
        {
            // maintains the slide speed
            rigid.linearVelocity = new Vector2(slideSpeed * transform.localScale.x, rigid.linearVelocity.y);

            if (Time.time > slideTime)
            {
                isSliding = false;

                // resets the rotations
                //transform.rotation = originalRotation;
                
                caps.size = originalColliderSize;
                caps.offset = originalColliderOffset;
                //caps.direction = CapsuleDirection2D.Vertical;

                /*
                box.size = originalBoxSize;
                box.offset = originalBoxOffset;
                */

                animator.SetBool("isSliding", false);
            }
        }
    }

    private bool IsGrounded()
    {
        
        Vector2 rayOriginLeft = new Vector2(caps.bounds.min.x, caps.bounds.center.y);
        Vector2 rayOriginRight = new Vector2(caps.bounds.max.x, caps.bounds.center.y);
        Vector2 rayOriginCenter = new Vector2(caps.bounds.center.x, caps.bounds.min.y);

        bool hitLeft = Physics2D.Raycast(rayOriginLeft, Vector2.down, rayLength, LayerMask.GetMask("ground"));
        bool hitRight = Physics2D.Raycast(rayOriginRight, Vector2.down, rayLength, LayerMask.GetMask("ground"));
        bool hitCenter = Physics2D.Raycast(rayOriginCenter, Vector2.down, rayLength, LayerMask.GetMask("ground"));

        return hitLeft || hitRight || hitCenter;
    }

    /*
    private bool IsNearWall()
    {
        Vector2 rayOrigin = caps.bounds.center;
        bool hitWall = Physics2D.Raycast(rayOrigin, Vector2.right * transform.localScale.x, rayLength, world);
        return hitWall;
    }
    */
}
