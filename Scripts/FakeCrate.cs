using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FakeCrate : Interactable
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private GameObject fakeKey;
    [SerializeField] private Light2D crateLight;
    [SerializeField] private float popUpHeight = 1.5f;
    [SerializeField] private float popUpDuration = 0.5f;
    [SerializeField] private float crateDestroyDelay = 1f;
    [SerializeField] private AudioClip[] crateOpenSounds;

    private bool keyAvailable = false;
    private bool isFakeKey = true;

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (fakeKey != null)
        {
            fakeKey.SetActive(false); // Initially keep the real key hidden
        }
    }

    protected override void Interact()
    {
        if (!keyAvailable)
        {
            // Play the crate opening sound
            if (audioSource != null && crateOpenSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, crateOpenSounds.Length);
                audioSource.PlayOneShot(crateOpenSounds[randomIndex]);
            }

            // Show the key (real or fake) and start the pop-up animation
            if (isFakeKey && fakeKey != null)
            {
                fakeKey.SetActive(true);
                StartCoroutine(PopUpKey(fakeKey));
            }
            keyAvailable = true;
        }
    }

    private IEnumerator PopUpKey(GameObject key)
    {
        Vector3 originalPosition = key.transform.position;
        Vector3 targetPosition = originalPosition + new Vector3(0f, popUpHeight, 0f);
        float elapsedTime = 0f;

        // Animate the key popping up
        while (elapsedTime < popUpDuration)
        {
            key.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / popUpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        key.transform.position = targetPosition;

        // Automatically destroy the crate after a delay
        yield return new WaitForSeconds(crateDestroyDelay);
        Destroy(gameObject);
    }
}
