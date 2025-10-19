using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MichaelJCrow : MonoBehaviour
{
    [SerializeField] private Transform player;
    
    [SerializeField] private float teleportCooldown = 5f;
    [SerializeField] private float teleportTriggerRange = 6f;
    private float teleportTimer = 0f;

    [SerializeField] private float chaseSpeed = 8f;
    [SerializeField] private float attackRange = 5f;

    [SerializeField] private float attackCooldown = 8f;
    [SerializeField] private float minAttackCooldown = 2f; 
    [SerializeField] private float cooldownReduction = 0.5f;

    [SerializeField] private int bossHealth = 150;
    [SerializeField] private int attackDamage = 8;

    [SerializeField] private float extraSpacing = 2.5f;
    [SerializeField] private float teleportDistance = 2f;

    [SerializeField] private Animator animator;
    [SerializeField] private playerHealth playerHP;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int currentHealth;
    [SerializeField] private HealthBar hp;
    private float lerpSpeed = 0.05f;

    private float attackTimer;
    private bool isAttacking = false;
    private bool canAttack = true;

    private bool canTeleport = true;
    public bool fightStarted = false;

    private bool isDefeated = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip teleportClip;
    [SerializeField] private AudioClip AttackClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    private void Start()
    {
        hp.SetSliderMax(bossHealth);
        currentHealth = bossHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!fightStarted || currentHealth <= 0)
        {
            if(isDefeated)
            {
                animator.SetBool("isRunning", false);
                return;
            }
            return;
        }

        attackTimer -= Time.deltaTime;
        teleportTimer -= Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Teleport if near player and cooldown has passed
        if (teleportTimer <= 0f && distanceToPlayer <= teleportTriggerRange)
        {
            TeleportBehindPlayer();
            PlaySound(teleportClip);
            teleportTimer = teleportCooldown; // Reset cooldown
        }

        if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
            isAttacking = true;
            StartCoroutine(Attack());
        }

        //decreases gradually to the attend health after damage
        if (hp.healthSLider.value != hp.delayedHealthSlider.value)
        {
            hp.delayedHealthSlider.value = Mathf.Lerp(hp.delayedHealthSlider.value, currentHealth, lerpSpeed);
        }
    }

    private void ChasePlayer()
    {
        //animator.SetBool("isRunning", true);

        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * chaseSpeed * Time.deltaTime);

        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        //animator.SetBool("isRunning", false);

        animator.SetTrigger("Attack");
        //PlaySound(AttackClip);
        yield return new WaitForSeconds(attackCooldown);

        attackCooldown = Mathf.Max(minAttackCooldown, attackCooldown - cooldownReduction);

        isAttacking = false;
    }

    public void DealDamage()
    {
        if (!fightStarted || currentHealth <= 0) return;

        // Check for player in range via tag and direction
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerHP.PlayerTakeDamage(attackDamage);
                break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animator.SetTrigger("Hurt");
        PlaySound(hurtClip);
        hp.SetSlider(currentHealth);
        
        if (currentHealth <= 0)
        {
            PrepareForDeath();
        }
    }

    private void PrepareForDeath()
    {
        isDefeated = true;
        fightStarted = false;
        animator.SetBool("isRunning", false);
        animator.Play("idle");
        StopAllCoroutines();
        Destroy(hp.gameObject);

        FindAnyObjectByType<CutSceneMangerMichaelJCrow>()?.StartBossDefeatedSequence();
    }

    public void Die()
    {
        fightStarted = false;
        animator.SetTrigger("Die");
        PlaySound(deathClip);
        StopAllCoroutines();

        
        Destroy(gameObject, 1.6f);
    }

    public void StartBossFight()
    {
        fightStarted = true;
        //StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        while (currentHealth > 0)
        {
        
            yield return new WaitForSeconds(teleportCooldown);

            //disables the collision
            //Collider2D bosscollider = GetComponent<Collider2D>();
           // bosscollider.enabled = false;

            TeleportBehindPlayer();

            yield return new WaitForSeconds(1f);
            //bosscollider.enabled = true;

            canAttack = true;
        }
    }

    private void TeleportBehindPlayer()
    {
        if (!fightStarted) return;

        // Determine player's facing direction using sprite flip instead of velocity
        bool isFacingRight = !player.GetComponentInChildren<SpriteRenderer>().flipX; // Assuming flipX = true means facing left
        float playerFacingDirection = isFacingRight ? 1 : -1;

        // Calculate teleport position behind the player
        float safeDistance = teleportDistance + extraSpacing;
        Vector2 teleportPosition = new Vector2(player.position.x - (safeDistance * playerFacingDirection), player.position.y);

        transform.position = teleportPosition;

        // Flip the boss to face the player
        spriteRenderer.flipX = player.position.x < transform.position.x;

        Debug.Log("Boss teleported behind the player.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, teleportTriggerRange); // Teleport trigger range
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
