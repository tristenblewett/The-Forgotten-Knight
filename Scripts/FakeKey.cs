using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeKey : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound; // Sound when key is collected
    [SerializeField] private float autoCollectDelay = 1f; // Delay before automatically collecting the key

    private void OnEnable()
    {
        // Start automatic collection after popping out
        StartCoroutine(AutoCollectKey());
    }

    private IEnumerator AutoCollectKey()
    {
        // Wait for a brief delay
        yield return new WaitForSeconds(autoCollectDelay);

        // Play collection sound if available
        if (audioSource != null && collectSound != null)
        {
            audioSource.PlayOneShot(collectSound);
        }

        // Wait a bit to let the sound play, then destroy the key
        yield return new WaitForSeconds(collectSound != null ? collectSound.length : 0f);

        Destroy(gameObject);
    }
}
