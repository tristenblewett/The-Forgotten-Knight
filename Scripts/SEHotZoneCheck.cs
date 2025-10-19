using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEHotZoneCheck : MonoBehaviour
{
    private SheildEnemy Senemy;
    private bool inRange;
    private Animator animator;

    private void Awake()
    {
        Senemy = GetComponentInParent<SheildEnemy>();
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if(inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("sheildattack"))
        {
            Senemy.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        { 

            inRange = false;
            gameObject.SetActive(false);
            Senemy.triggerArea.SetActive(true);
            
            Debug.Log("Player has left");
            Senemy.inRange = false;
            Senemy.SelectTarget();
        }
    }
}
