using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    private int LaserDamagetoPlayer = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerHealth PlayerHP = collision.GetComponent<playerHealth>();
            if(PlayerHP != null)
            {
                PlayerHP.PlayerTakeDamage(LaserDamagetoPlayer);
            }
        }
    }
}
