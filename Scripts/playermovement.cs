using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : ScriptableObject
{
    [Header("Layers")]

    public LayerMask playerLayer;

    [Header("Input")]

    public bool snapInput = true;
    public float verticalDeadZone = 0.3f;
    public float horizontalDeadZone = 0.1f;

    [Header("Movement")]

    public float maxSpeed = 14f;
    public float acceleration = 120f;
    public float groundDeceleration = 60f;
    public float airDeceleration = 30f;
    public float groundingForce = -1.5f;
    public float grounderDistance = 0.05f;

    [Header("Jump")]

    public float maxJumpHeight = 12f;
    public float jumpPower = 10f;
    public float doubleJumpPower = 7f;
    public float maxFallSpeed = -20f;
    public float fallAcceleration = 110f;
    public float jumpEndEarlyGravityModifier = 3f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.2f;

}
