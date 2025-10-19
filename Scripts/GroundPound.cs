using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPound : MonoBehaviour
{
    [SerializeField] private float stopTime = 1.5f; // Time to wait before dropping
    [SerializeField] private float dropForce = 10f; // Force applied during the ground pound
    [SerializeField] private float gravityScale = 1f; // Normal gravity scale after the ground pound

    private playercontroller player;
    private Rigidbody2D rigid;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform impactOrigin;
    [SerializeField] private int damage = 20;
    [SerializeField] private float impactRadius = 0.8f;

    private bool isGroundPounding = false;
    
    [SerializeField] private Animator animator;
    private void Awake()
    {
        player = GetComponent<playercontroller>();
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        player.DetectCollision();

        // Trigger the ground pound if the player is in the air and presses the mouse button
        if (Input.GetKeyDown(KeyCode.Mouse1) && !isGroundPounding && !player.isGround && player.jumpCount > 0)
        {
            StartGroundPound();
        }
    }
    /*
    private void FixedUpdate()
    {
        if (isGroundPounding)
        {
            GroundPoundAttack();
        }
    }
    */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the ground pound hits the ground or a surface
        if (collision.contacts[0].normal.y >= 0.5)
        {
            CompleteGroundPound();
        }
    }

    // Starts the ground pound sequence
    private void StartGroundPound()
    {
        isGroundPounding = true;
        animator.SetTrigger("isGroundPounding");
        StopAndSpin();
        
        StartCoroutine(DropAndSmash());
    }

    // Stop all movement and disable gravity temporarily for the ground pound preparation
    private void StopAndSpin()
    {
        ClearForces();
        rigid.gravityScale = 0;
    }

    // Coroutine to wait before applying the ground pound force
    private IEnumerator DropAndSmash()
    {
        yield return new WaitForSeconds(stopTime); // Wait for the delay
        rigid.gravityScale = gravityScale; // Restore gravity scale after waiting
        rigid.AddForce(Vector2.down * dropForce, ForceMode2D.Impulse); // Apply drop force
        isGroundPounding = false; // End the ground pound action
        DealDamage();
    }

    // Clears any velocity or forces on the rigidbody to stop movement
    private void ClearForces()
    {
        rigid.linearVelocity = Vector2.zero;
        rigid.angularVelocity = 0;
    }

    // Completes the ground pound, reset gravity to normal
    private void CompleteGroundPound()
    {
        rigid.gravityScale = gravityScale; // Restore gravity scale
        isGroundPounding = false; // End ground pound when hitting the ground

        
    }

    private void DealDamage()
    {
        if(impactOrigin == null)
        {
            return;
        }
        if (!isGroundPounding)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(impactOrigin.position, impactRadius, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {

                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                if (enemyComponent != null)
                {
                    enemyComponent.TakeDamage(damage);
                }

                SheildEnemy shieldEnemyComponent = enemy.GetComponent<SheildEnemy>();
                if (shieldEnemyComponent != null)
                {
                    if (shieldEnemyComponent.IsBlocking)
                    {

                        continue;
                    }
                    shieldEnemyComponent.TakeDamage(damage);
                }

                Slime slime = enemy.GetComponent<Slime>();
                if (slime != null)
                {
                    slime.TakeDamage(damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(impactOrigin == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(impactOrigin.position, impactRadius);
    }

}
