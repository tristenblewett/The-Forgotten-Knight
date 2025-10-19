using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private SpriteRenderer playerSprite;
    
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private int attackDamage = 10;
    //[SerializeField] private float attackSpeed = 1f;
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] swordSwingSound;
    [SerializeField] private AudioClip[] hitSound;
    [SerializeField] private AudioClip[] parrySound;

    private Vector3 originalAttackPointPosition;

    private void Start()
    {
        originalAttackPointPosition = attackPoint.localPosition;
    }

    private void Update()
    {
        FlipAttackPoint();
    }

    public void Attack()
    {
        PlaySwordSwingSound();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            PlayHitSound();
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(attackDamage);
            }

            SheildEnemy shieldEnemyComponent = enemy.GetComponent<SheildEnemy>();
            if (shieldEnemyComponent != null)
            {
                if(shieldEnemyComponent.IsBlocking)
                {
                    PlayRandomParrySound();
                    continue;
                }
                shieldEnemyComponent.TakeDamage(attackDamage);
            }

            Slime slime = enemy.GetComponent<Slime>();
            if(slime != null)
            {
                slime.TakeDamage(attackDamage);
            }

            RockBoss rBoss = enemy.GetComponent<RockBoss>();
            if(rBoss != null)
            {
                rBoss.TakeDamage(attackDamage);
            }

            PastKnightBoss knightBoss = enemy.GetComponent<PastKnightBoss>();
            if(knightBoss != null)
            {
                knightBoss.TakeDamage(attackDamage);
            }

            MichaelJCrow mjc = enemy.GetComponent<MichaelJCrow>();
            if (mjc != null)
            {
                mjc.TakeDamage(attackDamage);
            }

            FinalBoss finalBoss = enemy.GetComponent<FinalBoss>();
            if(finalBoss != null)
            {
                finalBoss.TakeDamage(attackDamage);
            }
        }
    }

    private void FlipAttackPoint()
    {
        if(playerSprite.flipX)
        {
            //flips left
            attackPoint.localPosition = new Vector3(-originalAttackPointPosition.x, originalAttackPointPosition.y, originalAttackPointPosition.x);
        }
        else
        {
            //flips right
            attackPoint.localPosition = originalAttackPointPosition; 
        }
    }

    public void PlaySwordSwingSound()
    {
       if(swordSwingSound.Length > 0)
        {
            int randomIndex = Random.Range(0, swordSwingSound.Length);
            AudioClip randomSwingSound = swordSwingSound[randomIndex];

            audioSource.PlayOneShot(randomSwingSound);
        }
    }

    public void PlayHitSound()
    {
        if(hitSound.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSound.Length);
            AudioClip randomHitSound = hitSound[randomIndex];

            audioSource.PlayOneShot(randomHitSound);
        }
    }

    private void PlayRandomParrySound()
    {
        if (parrySound.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, parrySound.Length);
            AudioClip randomParrySound = parrySound[randomIndex];
            audioSource.PlayOneShot(randomParrySound);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
