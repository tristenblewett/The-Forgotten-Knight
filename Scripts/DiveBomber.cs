using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveBomber : MonoBehaviour
{
    public Transform leftLimit;
    public Transform rightLimit;

    public float moveSpeed = 2f;
    public float diveSpeed = 8f;
    public float detectionRange = 5f;
    public float explosionRadius = 2f;
    public int damageToPlayer = 20;

    private Rigidbody2D rigid;
    [SerializeField] private Animator animator;
    private Transform player;
    private bool diving = false;
    private bool exploded = false;
    [SerializeField] public Transform target; // Patrol target

    public GameObject hotZone;
    public GameObject triggerArea;
    [HideInInspector] public bool inRange;


    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rigid.gravityScale = 0f;
    }

    private void Update()
    {
        if (exploded) return;

        if (!InsideofLimits() && !inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dash"))
        {
            SelectPatrolTarget();
        }

        if (!diving)
        {
            Patrol();

            if (Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                StartDive();
            }
        }
    }

    private void Patrol()
    {
        Vector2 targetPos = new Vector2(target.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        animator.SetTrigger("Patrolling");
        
    }

    private void SelectPatrolTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if (distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }
        Flip();
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    private void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.position.x)
            rotation.y = 180f;
        else
            rotation.y = 0f;
        transform.eulerAngles = rotation;
    }

    public void StartDive()
    {
        diving = true;
        rigid.gravityScale = 2f;
        rigid.linearVelocity = Vector2.zero; // Stop normal movement
        animator.SetTrigger("Dash"); // Optional dive animation
        StartCoroutine(DiveAtPlayer());
    }

    private IEnumerator DiveAtPlayer()
    {
        Vector2 diveTarget = player.position;

        while (!exploded)
        {
            transform.position = Vector2.MoveTowards(transform.position, diveTarget, diveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, diveTarget) <= 0.1f)
            {
                Explode();
            }

            yield return null;
        }
    }

    private void Explode()
    {
        exploded = true;

        // Play explosion animation if you have one
        animator.SetTrigger("Explode");

        // Deal damage to player if within radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<playerHealth>()?.PlayerTakeDamage(damageToPlayer);
            }
        }

        // Destroy DiveBomber after short delay
        Destroy(gameObject, 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!exploded && collision.collider.CompareTag("Player"))
        {
            Explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
