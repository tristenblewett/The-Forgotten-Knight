using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDamage : MonoBehaviour
{
    [SerializeField ]private int damage = 8;
    [SerializeField] private float knockBack = 3; // if i want to add this 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerHealth hp = collision.GetComponent<playerHealth>();
            Rigidbody2D rigid = collision.GetComponent<Rigidbody2D>();// for knockback if i want to add this in

            if(hp != null)
            {
                hp.PlayerTakeDamage(damage);
            }


        }
    }
}
