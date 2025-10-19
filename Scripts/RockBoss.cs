using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RockBoss : MonoBehaviour
{
    //fix the death timer, and the boss door 
    public bool isAwake = false;
    public float detectionRange = 10f;
    private GameObject player;
    public Transform playerTransform;
    private Rigidbody2D rigid;

    public Transform[] flyPoints;
    public float moveSpeed = 3f;
    private Transform currentTarget;

    public GameObject dagger;
    public Transform daggerPos;
    private float timer;

    public GameObject laserChargeUp;
    public GameObject laserShot;
    public Transform laserPivot;
    public Transform laserSpawnPoint;
    private float laserMaxRange = 10f;

    //public Transform attackPoint;
    public float attackCooldown = 5f;
    private float attackTimer;
    public float laserChargeDuration = 1.5f;
    private bool isAttacking = false;

    public float stunDuration = 4f;
    private bool isStunned = false;

    public int maxHealth = 500;
    private int currentHealth;
    [SerializeField] private HealthBar hp;
    private float lerpSpeed = 0.05f;

    public BossDoor doorController;

    private List<int> performedAttacks = new List<int>();

    //public GameObject bossSprite;
    [SerializeField] private SpriteRenderer bossFlip;

    public Animator bossAnimator; // Main boss animator
    public Animator daggerAnimator; // Dagger animation
    public Animator laserChargeAnimator; // Laser charge-up animation
    public Animator laserBeamAnimator; // Laser beam animation

    [SerializeField] private LayerMask blockingLayer;

    [SerializeField] private AudioSource bossSource;
    [SerializeField] private AudioClip laserChargeSound;
    [SerializeField] private AudioClip laserFireSound;
    [SerializeField] private AudioClip daggerShotSound;
    [SerializeField] private AudioClip bossWakeUpSound;
    [SerializeField] private AudioClip bossDeathSound;
    [SerializeField] private AudioClip laserBlockSound;

    private void Start()
    {
        isAwake = false;
        bossAnimator = GetComponent<Animator>();
        daggerAnimator = GetComponentInChildren<Animator>();
        laserChargeAnimator = GetComponentInChildren<Animator>();
        laserBeamAnimator = GetComponentInChildren<Animator>();
        bossSource = GetComponent<AudioSource>();
        //Door = GetComponent<BossDoor>();
        rigid = GetComponent<Rigidbody2D>();
        DeactivateAllAttackSprites();

        player = GameObject.FindGameObjectWithTag("Player");

        hp.SetSliderMax(maxHealth);
        currentHealth = maxHealth;

        if(blockingLayer == 0)
        {
            blockingLayer = LayerMask.GetMask("Pillar");
        }

    }

    private void Update()
    {
        if(!isAwake)
        {
            CheckPlayerProximity();
        }
        else
        {
            FlipBoss();
            //AimAtPlayer();
            MoveToTarget();
            HandleAttacks();
            //HandleDaggerAttack();
        }

        //decreases gradually to the attend health after damage
        if (hp.healthSLider.value != hp.delayedHealthSlider.value)
        {
            hp.delayedHealthSlider.value = Mathf.Lerp(hp.delayedHealthSlider.value, currentHealth, lerpSpeed);
        }
    }

    private void CheckPlayerProximity()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
            WakeUp();
        }
    }

    private void WakeUp()
    {
        isAwake = true;
        bossAnimator.SetTrigger("WakeUp");
        PlaySound(bossWakeUpSound);
        ChooseNextTarget();
    }

    private void MoveToTarget()
    {
        if (isAttacking) return;

        if(currentTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
            {
                ChooseNextTarget();
            }
        }
    }

    private void ChooseNextTarget()
    {
        if(flyPoints.Length > 0)
        {
            currentTarget = flyPoints[Random.Range(0, flyPoints.Length)];
        }
    }

    private void HandleAttacks()
    {
        if (isStunned) return; //doesnt attack when the boss get stunned

        // Reduce the attack timer
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            // Reset the attack cooldown
            attackTimer = attackCooldown;

            // Ensure we are not currently attacking
            if (isAttacking) return;

            // Clear performed attacks list to allow repeated attacks
            if (performedAttacks.Count >= 2)
            {
                performedAttacks.Clear();
            }

            // Determine the attack type
            int attackType;
            do
            {
                attackType = Random.Range(0, 2); // 0 for laser, 1 for dagger
            }
            while (performedAttacks.Contains(attackType));

            // Add the chosen attack to the performed attacks list
            performedAttacks.Add(attackType);

            // Execute the chosen attack
            if (attackType == 0)
            {
                // Perform the laser attack
                isAttacking = true;
                bossAnimator.SetTrigger("FireLaser");
                FireLaserBeam();
            }
        }

        // Always check if the dagger attack condition is met
        HandleDaggerAttack();

    }

    private void HandleDaggerAttack()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < 20)
        {
            timer += Time.deltaTime;

            if (timer > 2)
            {
                timer = 0;
                ShootDaggers();
            }
        }
    }

    private void ShootDaggers()
    {
        bossAnimator.SetTrigger("ShootDaggers");
        // Ensure the dagger GameObject is active
        ActivateSprite(dagger);
        PlaySound(daggerShotSound);

        Instantiate(dagger, daggerPos.position, Quaternion.identity);
    }


    private void FireLaserBeam()
    {
        ActivateSprite(laserChargeUp);
       
        Vector3 direction = GetDirectionToPlayer();
        float initialAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laserChargeAnimator.SetTrigger("Charge"); // Trigger charge-up animation
        PlaySound(laserChargeSound);

        Invoke(nameof(ShootLaser), laserChargeDuration);
    }

    private void ShootLaser()
    {
        DeactivateAllAttackSprites();
        ActivateSprite(laserShot);

        AimLaserAtPlayer();

        laserBeamAnimator.SetTrigger("Fire"); // Trigger laser beam animation
        PlaySound(laserFireSound);

        RaycastHit2D hit = Physics2D.Raycast(laserShot.transform.position, GetDirectionToPlayer(), Mathf.Infinity, blockingLayer);
        if (hit.collider != null)
        {
            Debug.DrawLine(laserShot.transform.position, hit.point, Color.blue, 2f);

            bossSource.Stop();
            PlaySound(laserBlockSound);

            ResetLaser(); // Reset laser instead of stopping
        }
      
        // Sweep logic
        float sweepAngleStart = laserShot.transform.rotation.eulerAngles.z;
        float sweepAngleEnd = sweepAngleStart + 45f;
        float sweepDuration = 2f;
   
        StartCoroutine(SweepLaser(laserShot.transform, sweepAngleStart, sweepAngleEnd,  sweepDuration));

        Invoke(nameof(EndAttack), sweepDuration + 0.5f);
    }

    private void ResetLaser()
    {
        // Reset laser position and any other necessary properties
        laserShot.transform.localScale = new Vector3(1f, 1f, 1f); // Reset scale
        laserShot.transform.position = laserSpawnPoint.position;  // Reset position to spawn point
        laserShot.transform.rotation = laserPivot.rotation;       // Reset rotation to default
        DeactivateAllAttackSprites();                             // Deactivate the laser visuals
    }

    private IEnumerator SweepLaser(Transform laserTransform, float sweepAngleStart, float sweepAngleEnd, float duration)
    {
        float time = 0f;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float currentSweepAngle = Mathf.Lerp(sweepAngleStart, sweepAngleEnd, t);

            // Combine the base angle with the sweep offset
            laserTransform.rotation = Quaternion.Euler(0, 0, currentSweepAngle);

            yield return null;
        }
    }

    private void EndAttack()
    {
        DeactivateAllAttackSprites();
        isAttacking = false;
    }

    private void ActivateSprite(GameObject sprite)
    {
        DeactivateAllAttackSprites();
        sprite.SetActive(true);
    }

    private void DeactivateAllAttackSprites()
    {
        dagger.SetActive(false);
        laserChargeUp.SetActive(false);
        laserShot.SetActive(false);
    }
    private Vector3 GetDirectionToPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        //Debug.Log($"Direction to player: {direction}");
        return direction;
    }

    private void FlipBoss()
    {
        Vector3 direction = GetDirectionToPlayer();
        // Flip the sprite based on the direction to the player
        bossFlip.flipX = direction.x < 0;
    }

    private void AimLaserAtPlayer()
    {
        Vector3 direction = (playerTransform.position - laserShot.transform.position).normalized;

        // Visualize the direction (for debugging purposes)
        Debug.DrawRay(laserShot.transform.position, direction * 5, Color.red, 1f);

        // Determine the angle towards the player based on the direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle to ensure the laser sweeps correctly based on the player's position
        if (laserShot.GetComponent<SpriteRenderer>().flipX) // Player is behind the boss (relative to the boss's facing direction)
        {
            angle += 180f; // Flip the angle to account for the laser facing the opposite direction
        }

        laserShot.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hp.SetSlider(currentHealth);
        bossAnimator.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Die();
            //DoorActvite();
        }
        else
        {
            StartCoroutine(StunCooldown());
        }
        
        
    }
    private void DoorActvite()
    {
        doorController.EnableInteraction();
        Debug.Log("door interaction enabled.");
    }
    private IEnumerator StunCooldown()
    {
        isStunned = true;
        isAttacking = true;
        moveSpeed = 0;
        yield return new WaitForSeconds(stunDuration);
        //bossAnimator.ResetTrigger("isHurt");
        moveSpeed = 3;
        isStunned = false;
        isAttacking = false;
    }

    private void Die()
    {
        bossAnimator.SetTrigger("Die");
        bossAnimator.ResetTrigger("isHurt");
        isAttacking = false;
        moveSpeed = 0f;
        rigid.linearVelocity = Vector2.zero;
        PlaySound(bossDeathSound);

        if (hp != null)
        {
            Destroy(hp.gameObject);

        }
       
        Destroy(gameObject, 1.8f);
        DoorActvite();
        

    }

    private void PlaySound(AudioClip clip)
    {
        if(clip != null && bossSource != null)
        {
            bossSource.PlayOneShot(clip);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        if (laserSpawnPoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 lastPoint = laserSpawnPoint.position;

            for (float angle = 90f; angle >= -90f; angle -= 5f)
            {
                Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                Vector3 nextPoint = laserSpawnPoint.position + direction * 2f;

                Gizmos.DrawLine(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }
        }
        
    }
}
