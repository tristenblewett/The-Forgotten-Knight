using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastKnightBoss : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float teleportDistance = 2f;
    private float teleportCooldown = 1.5f;
    private float attackRange = 4f;
    private float attackCooldown = 3f;
    [SerializeField] private float powerUpThreshold = 30f;
    [SerializeField] private float powerUpMultiplier = 1.5f;
    [SerializeField] private int bossHealth = 200;
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float extraSpacing = 2.5f;
    private float attackTimer = 0f;
    private bool isAttacking = false;

    [SerializeField] private Transform leftAttackArea;
    [SerializeField] private Transform rightAttackArea;
    //[SerializeField] private float attackRadius = 2f;

    private bool isPoweredUp = false;
    private bool isPoweringUp = false; // Flag to prevent actions
    private bool canAttack = true;

    [SerializeField] Animator animator;
    [SerializeField] private playerHealth playerHP;
    [SerializeField] SpriteRenderer spriteRenderer;

    private int currentHealth;
    [SerializeField] private HealthBar hp;
    private float lerpSpeed = 0.05f;

    public bool fightStarted = false;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip swordAttack;
    [SerializeField] private AudioClip teleport;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip powerUp;
    [SerializeField] private AudioClip damageSound;

    private void Start()
    {
        animator = GetComponent<Animator>();
        //playerHP = player.GetComponent<playerHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        

        hp.SetSliderMax(bossHealth);
        currentHealth = bossHealth;

        fightStarted = false;
    }

    private void Update()
    {
        if (!fightStarted) return;

        if (isPoweringUp)
        {
            Debug.Log("Boss is powering up, skipping Update logic.");
            return;
        }

        attackTimer -= Time.deltaTime;

        if(Vector2.Distance(transform.position, player.position) <= attackRange && canAttack && !isAttacking)
        {
            if (!isPoweringUp) // Make sure the attack doesn’t trigger during power-up
            {
                StartCoroutine(Attack());
            }
        }

        if(currentHealth <= powerUpThreshold && !isPoweredUp)
        {
            Debug.Log("Power-up condition met!");
            StartCoroutine(PowerUp());
        }

        //decreases gradually to the attend health after damage
        if (hp.healthSLider.value != hp.delayedHealthSlider.value)
        {
            hp.delayedHealthSlider.value = Mathf.Lerp(hp.delayedHealthSlider.value, currentHealth, lerpSpeed);
        }

        if (!fightStarted) return; // stop boss actoins if fight hasnt been triggerd
    }

    public void StartBossFight()
    {
        fightStarted = true;
        StartCoroutine(TeleportRoutine());
        Debug.Log("Fight has started!");
    }

    private IEnumerator TeleportRoutine()
    {
        while(currentHealth > 0)
        {
            if (isPoweringUp)
            {
                Debug.Log("Skipping teleport because boss is powering up.");
                yield break;
            }

            yield return new WaitForSeconds(teleportCooldown);

            //disables the collision
            Collider2D bosscollider = GetComponent<Collider2D>();
            bosscollider.enabled = false;

            TeleportBehindPlayer();
            PlaySound(teleport);
            yield return new WaitForSeconds(1f);
            bosscollider.enabled = true;

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

    private IEnumerator Attack()
    {
        if (!canAttack || isAttacking || isPoweringUp)
        {
            Debug.LogWarning("Tried to attack but boss is busy or powering up.");
            yield break;
        }

        canAttack = false;
        isAttacking = true;
        attackTimer = attackCooldown;

        Debug.Log("The boss is attacking.");

        animator.SetBool("Attack", true);
        PlaySound(swordAttack);
        //waiting for the attack animation to finish
        yield return new WaitForSeconds(2f);

        float attackDuration = 2f;
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            if (isPoweringUp)
            {
                Debug.Log("Attack interrupted by power-up!");
                animator.SetBool("Attack", false);
                isAttacking = false;
                
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        float extraAttackDelay = 3f;
        yield return new WaitForSeconds(attackCooldown + extraAttackDelay);
        canAttack = true;
        isAttacking = false;
    }

    public void DealDamage()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));

        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<playerHealth>().PlayerTakeDamage(attackDamage);
            Debug.Log("Boss hit the player!");
        }
        else
        {
            Debug.Log("Boss attack missed.");
        }
    }

    private IEnumerator PowerUp()
    {
        if (isPoweredUp) yield break;
        isPoweredUp = true;
        isPoweringUp = true;

        Debug.Log("Boss is powering up...");

        
        canAttack = false;
        isAttacking = false;

        // Make boss untargetable
        Collider2D bossCollider = GetComponent<Collider2D>();
        bossCollider.enabled = false;

        // Play power-up animation
        animator.SetBool("PowerUp", true);
        Debug.Log("Trying to play PowerUp animation: " + animator.GetCurrentAnimatorStateInfo(0).IsName("PowerUp"));
        PlaySound(powerUp);
        yield return new WaitForSeconds(3f); // Wait for animation to complete

        // Apply power-up effects
        teleportCooldown /= powerUpMultiplier;
        attackCooldown /= powerUpMultiplier;

        Debug.Log($"Power-up complete. New cooldowns - Teleport: {teleportCooldown}, Attack: {attackCooldown}");

        // Reactivate boss
        bossCollider.enabled = true;
        canAttack = true;
        isPoweringUp = false; // Allow actions again
        animator.SetBool("PowerUp", false);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hp.SetSlider(currentHealth);
        PlaySound(damageSound);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("The boss is defeated.");

        // Prevent all logic from continuing
        fightStarted = false;
        isAttacking = false;
        isPoweringUp = false;
        canAttack = false;

        // Stop any running coroutines
        StopAllCoroutines();

        // Turn off all animation flags
        animator.SetBool("Attack", false);
        PlaySound(death);
        //animator.SetTrigger("Die");

        if (hp != null)
        {
            Destroy(hp.gameObject);

        }

        Destroy(gameObject, 2f);

        FindAnyObjectByType<CutSceneMangerPastKnightBoss>().StartBossDefeatedSequence();
    }

    private void PlaySound(AudioClip clip)
    {
        if(clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
