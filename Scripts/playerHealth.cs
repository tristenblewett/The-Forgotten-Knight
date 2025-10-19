using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float lerpSpeed = 0.05f; //the speed of which the delays happen

    [Header("Death Settings")]
    [SerializeField] private Vector2 deathColliderSize = new Vector2(1f, 0.2f);
    [SerializeField] private Vector2 deathColliderOffset = new Vector2(0f, -0.25f);

    [Header("References")]
    [SerializeField] private Animator animator;
    public HealthBar healthBar;
    [SerializeField] private playeranimator playerAnimator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private float currentHealth;
    private Vector2 ogColliderSize;
    private Vector2 ogColliderOffset;
    private CapsuleCollider2D caps;
    //private Rigidbody2D rigid;
    private Vector2 startPos;
    private Vector2 respawnPos;

    private bool isHurt = false;
    public float hurtAnimationTime = 0.5f;

   // [SerializeField] private AudioSource audioSource;
   

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        caps = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponentInChildren<playeranimator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //rigid = GetComponent<Rigidbody2D>();

        ogColliderSize = caps.size;
        ogColliderOffset = caps.offset;
    }
    private void Start()
    {
        startPos = transform.position;
        respawnPos = startPos;

        currentHealth = maxHealth;

        healthBar.SetSliderMax(maxHealth);
    }

    private void Update()
    {
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        //when player dies
        if(currentHealth <=0)
        {
            Die();
        }

        //decreases gradually to the attend health after damage
        if (healthBar.healthSLider.value != healthBar.delayedHealthSlider.value)
        {
            healthBar.delayedHealthSlider.value = Mathf.Lerp(healthBar.delayedHealthSlider.value, currentHealth, lerpSpeed);
        }

        //testing
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            PlayerTakeDamage(10);
        }

    }

    public void PlayerTakeDamage(float health)
    {
        if (isHurt) return;

        currentHealth -= health;
        animator.SetTrigger("Hurt");
        healthBar.SetSlider(currentHealth);

        StartCoroutine(HurtCooldown());
    }

    private IEnumerator HurtCooldown()
    {
        isHurt = true;
        yield return new WaitForSeconds(hurtAnimationTime);
        isHurt = false;
    }

    public void Heal(float health)
    {
        currentHealth += health;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar.SetSlider(currentHealth);
        Debug.Log("The player has been healed");
    }

    public bool IsAtFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    public void Die()
    {
        FindAnyObjectByType<FinalBoss>()?.OnplayerDeath();

        caps.size = deathColliderSize;
        caps.offset = deathColliderOffset;

        //rigid.gravityScale = 1;// makes it fall through the ground 

        GetComponent<playercontroller>().enabled = false;
        animator.SetTrigger("Die");

        StartCoroutine(Respawn(1f));
        
    }

    //fix the footstep sound, it will go crazy when the player respawns
    private IEnumerator Respawn(float duration)
    {
        playercontroller playerController = GetComponent<playercontroller>();

        playerController.audioSource.mute = true;
        playerController.enabled = false;

        yield return new WaitForSeconds(duration);

        spriteRenderer.enabled = false;

        transform.position = respawnPos; //respawns back at the og spot that the player started
        
        //resets the colliders
        caps.size = ogColliderSize;
        caps.offset = ogColliderOffset;

        
        
        if(playerController != null)
        {
            playerController.RespawnAtCheckpoint();
        }
        
        currentHealth = maxHealth;//restores the health of the player
        healthBar.SetSlider(currentHealth);

        
        //playerAnimator.HandleIdleSpeed();// reset the animator back to the idle pos
        


        CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();

        if(cam != null)
        {
            cam.Follow = transform; //sets the camera to follow the player
        }

        yield return new WaitForSeconds(1.2f);

        playerController.audioSource.mute = false;
        playerController.enabled = true;

        spriteRenderer.enabled = true;

        animator.ResetTrigger("Die");
        animator.Play("idle");
    }

    public void SetRespawnPosition(Vector2 newRespawnPos)
    {
        respawnPos = newRespawnPos;
    }

}
