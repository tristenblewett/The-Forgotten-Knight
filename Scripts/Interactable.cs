using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected float interactionRange = 1f;
    [SerializeField] private LayerMask playerLayer;

    private bool isPlayerNearby = false;

    protected virtual void Update()
    {
        if(isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed, interacting with the object.");
            Interact();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player entered interaction range.");
            isPlayerNearby = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    protected abstract void Interact();
}
