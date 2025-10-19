using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheildEnemy : MonoBehaviour
{
    public Transform leftLimit;
    public Transform rightLimit;
  
    public float attackDistance;
    public float moveSpeed;
    public float timer;

    public int maxHealth = 100;
    public int currentHealth;
    public int damageToPlayer = 5;
    public float hurtAnimationTime = 0.5f;

    public GameObject hotZone;
    public GameObject triggerArea;

    [SerializeField] public HealthBar hp;
    [SerializeField] private float lerpSpeed = 0.05f;

    private bool isDead = false;

    private Rigidbody2D rigid;
   [HideInInspector] public Transform target;
    private Animator animator;
    private float distance;
    private bool attackMode;
    [HideInInspector] public bool inRange;
    private bool cooldown;
    private float intTimer;

    public float blockCooldown = 3f; // Time between blocks
    public float blockDuration = 1.5f; // How long the block lasts
    public float blockChance = 0.5f; // Chance of blocking an attack
    private bool isBlocking = false;
    private bool canBlock = true; // Can the enemy currently block
    public bool IsBlocking => isBlocking; // Expose the blocking state

    private bool isHurt = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] swordSwingSound;
    [SerializeField] private AudioClip[] walkingSound;
    [SerializeField] private AudioClip[] hitSound;

    private float walkSoundCooldown = 0.5f; // Time between walk sounds
    private float walkSoundTimer;
    [SerializeField] private float attackSoundCooldown = 1f;
    private float attackSoundTimer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        //hp = GetComponent<HealthBar>();

        intTimer = timer;
        currentHealth = maxHealth;
        hp.SetSliderMax(maxHealth);
        SelectTarget();
        attackSoundTimer = 0;
        walkSoundTimer = walkSoundCooldown; // Initialize the walk sound timer
    }

    private void Update()
    {

        // Update the walk sound timer
        if (walkSoundTimer > 0)
        {
            walkSoundTimer -= Time.deltaTime;
        }

        if (!attackMode)
        {
            Move();
        }

        if (!InsideofLimits() && !inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("sheildattack"))
        {
            SelectTarget();
        }
        

        if (inRange)
        {
            EnemyLogic();
        }

        // Handle blocking logic
        if (canBlock && !isBlocking && Random.value < blockChance)
        {
            StartCoroutine(Block());
        }

        //decreases gradually to the attend health after damage
        if (hp.healthSLider.value != hp.delayedHealthSlider.value)
        {
            hp.delayedHealthSlider.value = Mathf.Lerp(hp.delayedHealthSlider.value, currentHealth, lerpSpeed);
        }
        
        if(attackSoundTimer >0)
        {
            attackSoundTimer -= Time.deltaTime;
        }

    }

    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (attackDistance >= distance && cooldown == false)
        {
            Attack();
        }

        if (cooldown)
        {
            Cooldown();
            animator.SetBool("Attack", false);
        }
    }

    private void Cooldown()
    {
        timer -= Time.deltaTime;

        if(timer <= 0 && cooldown && attackMode)
        {
            cooldown = false;
            timer = intTimer;
        }
    }

    private void Move()
    {
        animator.SetBool("isWalking", true);
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("sheildattack"))
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Play walk sound if cooldown is up
            if (inRange && walkSoundTimer <= 0)
            {
                PlayWalkSound();
            }
        }
    }

    private void Attack()
    {
        if (cooldown) return;
       
        timer = intTimer;
        attackMode = true;

        animator.SetBool("isWalking", false);
        animator.SetBool("Attack", true);

        PlaySwordSwingSound();
    }

    public void DealingDamageToPlayer()
    {
        if (target != null && target.CompareTag("Player"))
        {
            target.GetComponent<playerHealth>().PlayerTakeDamage(damageToPlayer);
            PlayHitSound();
        }
    }

    // Block coroutine to manage block timing
    private IEnumerator Block()
    {
        isBlocking = true;
        canBlock = false; // Temporarily disable blocking

        // Trigger block animation
        animator.SetTrigger("Block");

        // Stay in block mode for blockDuration
        yield return new WaitForSeconds(blockDuration);

        // Exit block mode
        isBlocking = false;

        // Start cooldown before being able to block again
        yield return new WaitForSeconds(blockCooldown);
        canBlock = true;
    }

    private void StopAttack()
    {
        cooldown = false;
        attackMode = false;
        animator.SetBool("Attack", false);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (isBlocking)
        {
            return;
        }
        currentHealth -= damage;
        animator.SetTrigger("Damage");
        hp.SetSlider(currentHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(HurtCoolDown());
        }
    }

    private IEnumerator HurtCoolDown()
    {
        isHurt = true;
        moveSpeed = 0;
        yield return new WaitForSeconds(hurtAnimationTime);
        moveSpeed = 2;
        isHurt = false;
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        animator.ResetTrigger("Damage");
        rigid.linearVelocity = Vector2.zero;
        
        if (hp != null)
        {
            Destroy(hp.gameObject);
        }
        
        this.enabled = false;

        Destroy(gameObject, 0.6f);
    }


    public void TriggerCoolDown()
    {
        cooldown = true;
        //attackMode = false;
    }

    private bool InsideofLimits()
    {
        return transform.position.x > leftLimit.position.x && transform.position.x < rightLimit.position.x;
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftLimit.position);
        float distanceToRight = Vector2.Distance(transform.position, rightLimit.position);

        if(distanceToLeft > distanceToRight)
        {
            target = leftLimit;
        }
        else
        {
            target = rightLimit;
        }
        Flip();
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if(transform.position.x > target.position.x)
        {
            rotation.y = 180f;
        }
        else
        {
            rotation.y = 0f;
        }
        transform.eulerAngles = rotation;
    }

    private void PlaySwordSwingSound()
    {
        // Only play sound if the cooldown has expired
        if (attackSoundTimer <= 0)
        {
            // Select a random attack sound from the array (assuming you have it defined)
            AudioClip randomAttackSound = swordSwingSound[Random.Range(0, swordSwingSound.Length)];
            audioSource.PlayOneShot(randomAttackSound);

            // Reset cooldown timer
            attackSoundTimer = attackSoundCooldown;
        }
    }

    private void PlayWalkSound()
    {
        // Select a random walking sound from the array
        AudioClip randomWalkSound = walkingSound[Random.Range(0, walkingSound.Length)];
        audioSource.PlayOneShot(randomWalkSound);

        // Reset the walk sound timer
        walkSoundTimer = walkSoundCooldown;
    }
   
    private void PlayHitSound()
    {
        if (hitSound.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSound.Length);
            AudioClip randomHitSound = hitSound[randomIndex];

            audioSource.PlayOneShot(randomHitSound);
        }
    }

}
