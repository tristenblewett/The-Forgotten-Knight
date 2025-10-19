using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeTriggerAreaCheck : MonoBehaviour
{
    public Slime slime; // Reference to the Slime script

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            slime.SetPlayerDetected(true); // Notify slime about player detection
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            slime.SetPlayerDetected(false); // Notify slime about player leaving
        }
    }
}
