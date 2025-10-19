using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    public float moveSpeed;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    [SerializeField] GameObject[] wayPoints;

    public int maxHealth = 4; // Slime's maximum health

    private Animator animator;

    public int damageToPlayer = 5;
    public Transform target;

    private int currentHealth;
    private int nextWayPoint = 1;
    private float distToPoint;
    private bool playerDetected = false;
    private float lastAttackTime = -Mathf.Infinity;


    private void Start()
    {
        currentHealth = maxHealth; // Set initial health
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (playerDetected)
        {
            AttackPlayer();
        }
        else
        {
            Move();
        }
    }

    private void Move()
    {
        distToPoint = Vector2.Distance(transform.position, wayPoints[nextWayPoint].transform.position);

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[nextWayPoint].transform.position, moveSpeed * Time.deltaTime);

        animator.SetBool("isWalking", true);

        if(distToPoint < 0.2f)
        {
            TakeTurn();
        }
    }

    private void TakeTurn()
    {
        Vector3 currRot = transform.eulerAngles;
        currRot.z += wayPoints[nextWayPoint].transform.eulerAngles.z;
        transform.eulerAngles = currRot;
        ChooseNextWaypoint();
    }

    private void ChooseNextWaypoint()
    {
        nextWayPoint++;

        if(nextWayPoint == wayPoints.Length)
        {
            nextWayPoint = 0;
        }
    }

    private void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);

            // Implement the attack logic here, e.g., deal damage to the player
            Debug.Log("Slime attacks the player!");
            lastAttackTime = Time.time;
        }
    }

    public void DealingDamageToPlayer()
    {
        if (target != null && target.CompareTag("Player"))
        {
            target.GetComponent<playerHealth>().PlayerTakeDamage(damageToPlayer);
        }
    }

    // Method to set playerDetected from the PlayerDetection script
    public void SetPlayerDetected(bool isDetected)
    {
        playerDetected = isDetected;

        if(!isDetected)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Slime takes damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("isHurt");
        }
    }

    private void Die()
    {
        Debug.Log("Slime has died!");

        animator.SetTrigger("isDead");
        // Add any additional death effects, animations, or sound here
        Destroy(gameObject); // Destroys the slime GameObject
    }
}
