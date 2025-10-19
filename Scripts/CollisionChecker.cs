using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallCheckDistanceRight;
    [SerializeField] private float wallCheckDistanceLeft;
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    
    [SerializeField] private LayerMask whatisGround;
    public bool isGrounded;

    [Header("Ledge Detection Settings")]
    [SerializeField] private Transform ledgeCheckLeft;
    [SerializeField] private Transform ledgeCheckRight;
    [SerializeField] private float ledgeCheckRadius = 0.5f;
    public bool ledgeDetectedLeft;
    public bool ledgeDetectedRight;
    

    public bool isWallDetectedLeft;
    public bool isWallDetectedRight;

    private void Update()
    {
        CollisionCheck();
    }

    private void CollisionCheck()
    {
        Vector2 directionLeft = Vector2.left;
        Vector2 directionRight = Vector2.right;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatisGround);
        isWallDetectedLeft = Physics2D.Raycast(wallCheck.position, directionLeft, wallCheckDistanceLeft, whatisGround);
        isWallDetectedRight = Physics2D.Raycast(wallCheck.position, directionRight, wallCheckDistanceRight, whatisGround);

        ledgeDetectedLeft = Physics2D.OverlapCircle(ledgeCheckLeft.position, ledgeCheckRadius, whatisGround);
        ledgeDetectedRight = Physics2D.OverlapCircle(ledgeCheckRight.position, ledgeCheckRadius, whatisGround);

        /*
        if (isWallDetectedLeft)
            Debug.Log("Wall Detected on the Left");

        if (isWallDetectedRight)
            Debug.Log("Wall Detected on the Right");
        if (isGrounded)
            Debug.Log("Ground");
        */

        //Debug.Log($"Ledge Detected Left: {ledgeDetectedLeft}, Right: {ledgeDetectedRight}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.left * wallCheckDistanceLeft);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * wallCheckDistanceRight);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(ledgeCheckLeft.position, ledgeCheckRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(ledgeCheckRight.position, ledgeCheckRadius);
    }
}
