using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitbox : MonoBehaviour
{
    public int attackDamage = 20;  // Damage dealt by the hitbox
    private bool hasHitPlayer = false;  // Ensure player is only hit once per attack
   //[SerializeField] public BoxCollider2D attackHitbox;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasHitPlayer)
        {
            playerHealth playerHealth = collision.GetComponent<playerHealth>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(attackDamage);  // Apply damage
                hasHitPlayer = true;  // Mark the hit as done
            }
        }
    }

    // Reset hasHitPlayer after attack ends
    public void ResetHitbox()
    {
        hasHitPlayer = false;
    }
}
