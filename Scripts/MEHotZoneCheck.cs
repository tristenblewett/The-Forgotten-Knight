using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MEHotZoneCheck : MonoBehaviour
{
    private Enemy Menemy;
    private bool inRange;
    private Animator animator;

    private void Awake()
    {
        Menemy = GetComponentInParent<Enemy>();
        animator = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if (inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName("sheildattack"))
        {
            Menemy.Flip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            inRange = false;
            gameObject.SetActive(false);
            Menemy.triggerArea.SetActive(true);

            Debug.Log("Player has left");
            Menemy.inRange = false;
            Menemy.SelectTarget();
        }
    }
}
