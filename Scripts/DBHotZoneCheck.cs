using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBHotZoneCheck : MonoBehaviour
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
            Debug.Log("Player entered hot zone");

            diveBomber.StartDive(); // Immediately start diving attack
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player left hot zone");

            diveBomber.inRange = false;
            diveBomber.hotZone.SetActive(false);
            diveBomber.triggerArea.SetActive(true); // Reactivate trigger area if needed
        }
    }
}
