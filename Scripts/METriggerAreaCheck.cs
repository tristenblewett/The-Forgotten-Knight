using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class METriggerAreaCheck : MonoBehaviour
{
    private Enemy Menemy;

    private void Awake()
    {
        Menemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player has enter ");
            gameObject.SetActive(false);
            Menemy.target = collision.transform;
            Menemy.inRange = true;
            Menemy.hotZone.SetActive(true);
        }
    }
}
