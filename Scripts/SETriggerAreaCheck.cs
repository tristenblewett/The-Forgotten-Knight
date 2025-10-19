using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SETriggerAreaCheck : MonoBehaviour
{
    private SheildEnemy Senemy;

    private void Awake()
    {
        Senemy = GetComponentInParent<SheildEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has enter ");
            gameObject.SetActive(false);
            Senemy.target = collision.transform;
            Senemy.inRange = true;
            Senemy.hotZone.SetActive(true);
        }
    }
}
