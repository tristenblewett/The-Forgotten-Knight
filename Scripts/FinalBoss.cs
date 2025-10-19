using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FinalBoss : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private playerHealth playerHP;
    [SerializeField] private SpriteRenderer spriteRender;

    [SerializeField] private int bossHealth = 400;
    [SerializeField] private int attackDamage = 4;
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackCooldown = 3.5f;
    private bool attackMode;

    private float dashTimer = 0f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashCooldown = 3f;
    private bool isDashing = false;

    [SerializeField] private float heavyAttackRadius = 6f;
    [SerializeField] private int heavyAttackDamage = 8;
    [SerializeField] private float heavyAttackCooldown = 6f;
    [SerializeField] private float heavyattackChance = 0.8f;
    private bool canHeavyAttack = true;

    private int currentHealth;
    [SerializeField] private HealthBar hp;
    private float lerpSpeed = 0.05f;

    private float decisionTimer = 0f;   // how often boss makes a decision
    [SerializeField] private float decisionInterval = 1.5f; // every 1.5s

    private float attackTimer = 0f;
    private bool cooldown;
    public float timer;
    private float intTimer;

    private bool isAttacking = false;
    private bool canAttack = true;

    [SerializeField] private float blockCooldown = 4f; // Time between blocks
    [SerializeField] private float blockDuration = 1.5f; // How long the block lasts
    [SerializeField] private float blockChance = 1f; // Chance of blocking an attack
    private bool isBlocking = false;
    private bool canBlock = true; // Can the enemy currently block

    public bool fightStarted = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip swordAttack;
    [SerializeField] private AudioClip heavyAttack;
    [SerializeField] private AudioClip rewardSound;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip teleportSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip weakenBossSound;
    [SerializeField] private AudioClip powerupBosssound;

    [SerializeField] private Transform bossPhaseTwo;
    [SerializeField] private Transform playerPhaseTwo;
    private bool hasPhaseTwoTriggered = false;

    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private GameObject playerObject;

    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject log;
    [SerializeField] private int enemiesToActivate = 2;
    [SerializeField] private GameObject healthCrate;
    [SerializeField] private int bosshpLoss = 100;
    private bool tunnelSwquenceActive = false;
    private bool playerDiedinTrap = false;
    private Rigidbody2D rigid;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody2D>();

        hp.SetSliderMax(bossHealth);
        currentHealth = bossHealth;

        intTimer = timer;
        fightStarted = false;
    }

    private void Update()
    {
        if (!fightStarted) return;

        attackTimer -= Time.deltaTime;
        dashTimer -= Time.deltaTime;
        decisionTimer -= Time.deltaTime;

        if (cooldown)
        {
            Cooldown();
        }

        if (decisionTimer <= 0f && !isAttacking && !isBlocking && !isDashing)
        {
            decisionTimer = decisionInterval; // reset

            float distance = Vector2.Distance(transform.position, player.position);


            if (canHeavyAttack && distance <= heavyAttackRadius && Random.value < heavyattackChance)
            {
                HeavyAttack();
            }
            else if (canAttack && !cooldown && distance <= attackRange)
            {
                Attack();
            }
            else if (canBlock && Random.value < blockChance)
            {
                StartCoroutine(Block());
            }
        }
       
        // Dash towards player if off cooldown
        if (!isDashing && dashTimer <= 0f && !isBlocking && !isAttacking)
        {
            StartCoroutine(DashTowardsPlayer());
        }

        //decreases gradually to the attend health after damage
        if (hp.healthSLider.value != hp.delayedHealthSlider.value)
        {
            hp.delayedHealthSlider.value = Mathf.Lerp(hp.delayedHealthSlider.value, currentHealth, lerpSpeed);
        }
    }

    public void StartBossFight()
    {
        fightStarted = true;
        Debug.Log("Final Boss fight started!");
    }

    private IEnumerator DashTowardsPlayer()
    {
        isDashing = true;
        dashTimer = dashCooldown;

        canAttack = false;
        isAttacking = false;

        Vector2 direction = new Vector2((player.position.x - transform.position.x), 0).normalized;

        animator.SetBool("Dash", true);
        PlaySound(dashSound);

        float dashDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            transform.position += (Vector3)(direction * dashSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRender.flipX = player.position.x < transform.position.x;

        animator.SetBool("Dash", false);
        isDashing = false;
        canAttack = true;
    }

    private void Attack()
    {
        if (!canAttack || isAttacking || cooldown) return;

        canAttack = false;
        isAttacking = true;
        cooldown = true;

        attackMode = true;
        animator.SetBool("Attack", true);
        PlaySound(swordAttack);
        ToggleCollider(true);

        Invoke(nameof(PerformAttack), 0.5f);

        StartCoroutine(ResetAttack());
    }

    private void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooldown && attackMode)
        {
            cooldown = false;
            attackMode = false;
            timer = intTimer;
        }
    }

    private void PerformAttack()
    {
        DealDamage();
        ToggleCollider(false); // turn collider back off after strike
    }

    private IEnumerator ResetAttack()
    {
        // wait for attack animation duration
        yield return new WaitForSeconds(0.8f);
        isAttacking = false;
        animator.SetBool("Attack", false);

        canAttack = true;
    }

    private void HeavyAttack()
    {
        if (!canHeavyAttack || isAttacking || isDashing || cooldown) return;

        canHeavyAttack = false;
        isAttacking = true;
        cooldown = true;
        timer = heavyAttackCooldown;

        attackMode = true;
        animator.SetBool("HeavyAttack", true);
        PlaySound(heavyAttack);
        //ToggleCollider(true);

        Invoke(nameof(PerformHeavyAttack), 0.8f);
        StartCoroutine(ResetHeavyAttack());
    }

    private void PerformHeavyAttack()
    {
        DealDamage();  // optionally use heavyAttackDamage instead of regular attackDamage
        //ToggleCollider(false);
    }

    private IEnumerator ResetHeavyAttack()
    {
        // wait for animation duration
        yield return new WaitForSeconds(0.8f);
        isAttacking = false;
        animator.SetBool("HeavyAttack", false);

        canAttack = true;
    }

    // Block coroutine to manage block timing
    private IEnumerator Block()
    {
        isBlocking = true;
        canBlock = false; // Temporarily disable blocking

        // Trigger block animation
        animator.SetBool("Block", true);
        ToggleCollider(true);
        // Stay in block mode for blockDuration
        yield return new WaitForSeconds(blockDuration);

        // Exit block mode
        isBlocking = false;
        animator.SetBool("Block", false);

        // Start cooldown before being able to block again
        yield return new WaitForSeconds(blockCooldown);
        canBlock = true;
        ToggleCollider(false);
    }

    public void StartTunnelEnemyWave()
    {
        if (tunnelSwquenceActive) return;

        tunnelSwquenceActive = true;

        fightStarted = false;
        canAttack = false;
        isAttacking = false;

        StartCoroutine(HandleTunnelWave());
    }

    private IEnumerator HandleTunnelWave()
    {

        yield return new WaitForSeconds(2f);

        ActivateRandomEnemies(2);

        yield return new WaitUntil(() => AllTunnelEnemiesDefeated());

        yield return new WaitForSeconds(2f);

        ActivateRandomEnemies(2);

        yield return new WaitUntil(() => AllTunnelEnemiesDefeated());

        yield return new WaitForSeconds(2f);

        ActivateRandomEnemies(2);

        yield return new WaitUntil(() => AllTunnelEnemiesDefeated());

        yield return new WaitForSeconds(1f);

        if(log != null)
        {
            log.SetActive(false);
        }

        if (healthCrate != null)
        {
            healthCrate.SetActive(true);

            PlaySound(rewardSound);
        }

        // Damage boss as part of the event
        TakeDamage(bosshpLoss);
        WeakenBoss();

        if(playerDiedinTrap)
        {
            PowerUpBoss();
            playerDiedinTrap = false;
        }

        fightStarted = true;
        canAttack = true;
        tunnelSwquenceActive = false;
    }

    private void ActivateRandomEnemies(int count)
    {
        List<int> availableIndices = new List<int>();

        // Collect all inactive enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && !enemies[i].activeInHierarchy)
                availableIndices.Add(i);
        }

        // If we have fewer inactive enemies than requested, cap it
        int spawnCount = Mathf.Min(count, availableIndices.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            int randIndex = Random.Range(0, availableIndices.Count);
            int chosen = availableIndices[randIndex];
            availableIndices.RemoveAt(randIndex);

            enemies[chosen].SetActive(true);
            Debug.Log("Spawned enemy: " + enemies[chosen].name);
        }
    }

    private bool AllTunnelEnemiesDefeated()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
                return false;
        }
        return true;
    }

    public void OnplayerDeath()
    {
        if(tunnelSwquenceActive)
        {
            playerDiedinTrap = true;
        }
    }

    private void WeakenBoss()
    {
        PlaySound(weakenBossSound);
        int reducedDamage = 2;
        float slowerSpeed = 2f;
        float slowerDescision = 0.2f;

        attackDamage = Mathf.Max(1, attackDamage - reducedDamage);
        dashSpeed = Mathf.Max(5f, dashSpeed - slowerSpeed);
        decisionInterval += slowerDescision;
    }

    private void PowerUpBoss()
    {
        PlaySound(powerupBosssound);
        int extraHealth = 150;
        int extraDamage = 2;

        bossHealth += extraHealth;
        currentHealth += extraHealth;
        attackDamage += extraDamage;

        dashSpeed += 3f;
        decisionInterval = Mathf.Max(0.8f, decisionInterval - 0.2f);

        hp.SetSliderMax(bossHealth);
        hp.SetSlider(currentHealth);
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hp.SetSlider(currentHealth);
        PlaySound(damageSound);
        animator.SetTrigger("Hurt");
          
        if(!hasPhaseTwoTriggered && currentHealth <= bossHealth / 2)
        {
            StartCoroutine(TriggerPhaseTwo());
        }

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator TriggerPhaseTwo()
    {
        hasPhaseTwoTriggered = true;
        Debug.Log("Boss has entered Phase Two");

        fightStarted = false;
        canAttack = false;
        isAttacking = false;

        PlaySound(teleportSound);

        yield return new WaitForSeconds(2f);

        GameObject playertp = playerObject;

        if (bossPhaseTwo != null)
        {
            transform.position = bossPhaseTwo.position;
            
        }

        if(playerPhaseTwo != null)
        {
            playertp.transform.position = playerPhaseTwo.position;
            UpdateCameraFollow(playertp.transform);

            playercontroller playerController = playertp.GetComponent<playercontroller>();
            if(playerController != null)
            {
                playerController.SetCheckpoint(playerPhaseTwo.position);
                Debug.Log("player new spawn point has been set");
            }
        }

        FaceTarget(player.transform);

        PlayerFaceTarget(player.transform, transform);

        yield return new WaitForSeconds(0.8f);

        FindAnyObjectByType<CutSceneMangerFinal>().StartAfterTeleportBossCutScene();
        //fightStarted = true;
        //canAttack = true;
    }

    private void UpdateCameraFollow(Transform playerTransform)
    {
        CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
        if (cam != null)
        {
            cam.Follow = playerTransform;
            Debug.Log("Camera follow updated to: " + playerTransform.name);
        }
        else
        {
            Debug.LogWarning("No CinemachineVirtualCamera found in the scene!");
        }
    }

    private void FaceTarget(Transform target)
    {
        if(spriteRender != null)
        {
            spriteRender.flipX = target.position.x < transform.position.x;
        }
    }

    private void PlayerFaceTarget(Transform player, Transform boss)
    {
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if(playerSprite != null)
        {
            playerSprite.flipX = boss.position.x < player.position.x;
        }
    }

    private void Die()
    {
        fightStarted = false;
        isAttacking = false;
        canAttack = false;

        animator.SetTrigger("Die");
        PlaySound(deathSound);

        StopAllCoroutines();

        if (hp != null)
        {
            Destroy(hp.gameObject);

        }

        Destroy(gameObject, 2f);
        FindAnyObjectByType<CutSceneMangerFinal>().TheEndingSequence();
    }

    private void ToggleCollider(bool state)
    {
        Collider2D col = GetComponent<Collider2D>();
        if(col != null)
        {
            col.enabled = state;
        }

        if(!state)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
    private void PlaySound(AudioClip clip)
    {
        if(clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack range circle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
