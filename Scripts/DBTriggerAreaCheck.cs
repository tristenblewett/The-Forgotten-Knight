using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBTriggerAreaCheck : MonoBehaviour
{
    private DiveBomber diveBomber;

    private void Awake()
    {
        diveBomber = GetComponentInParent<DiveBomber>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger area");

            gameObject.SetActive(false); // Disable trigger area
            diveBomber.target = collision.transform; // Assign player as target
            diveBomber.inRange = true;
            diveBomber.hotZone.SetActive(true); // Activate HotZone
        }
    }
}
