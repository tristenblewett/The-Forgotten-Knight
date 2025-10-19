using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class checkpoint : Interactable
{
    [Header("Checkpoint Settings")]
    [SerializeField] private Animator firePitAnimator;
    [SerializeField] private Light2D firePitLight;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float maxVolume = 1f;

    private Vector2 checkpointPosition;
    private playercontroller playerController;

    private void Awake()
    {
        firePitAnimator = GetComponentInChildren<Animator>();
        firePitLight = GetComponentInChildren<Light2D>();
        checkpointPosition = transform.position;
        playerController = FindObjectOfType<playercontroller>();

        if(firePitLight != null)
        {
            firePitLight.enabled = false;
        }

        if(audioSource != null)
        {
            audioSource.volume = 0f;
            audioSource.loop = true;
        }
    }

    protected override void Interact()
    {
        ActivateCheckpoint();
    }

    private void ActivateCheckpoint()
    {

        if (playerController != null)
        {
            playerController.TriggerPrayAnimation();
            firePitAnimator.SetTrigger("Ignite");

            if(audioSource != null & activationSound !=null)
            {
                audioSource.clip = activationSound;
                audioSource.Play();
                StartCoroutine(FadeInSound(audioSource, fadeDuration, maxVolume));
            }
            StartCoroutine(HandlePostActivation());
        }

    }

    private IEnumerator HandlePostActivation()
    {
        yield return new WaitForSeconds(1f);

        playerController.GetComponentInChildren<Animator>().Play("idle");

        if(firePitLight != null)
        {
            firePitLight.enabled = true;
        }

        playerController.SetCheckpoint(checkpointPosition);
        StartCoroutine(FadeOutSound(audioSource, fadeDuration));
    }

    private IEnumerator FadeInSound(AudioSource source, float duration, float targetVolume)
    {
        float currentTime = 0;
        float startVolume = source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        source.volume = targetVolume;
    }

    // Coroutine to fade out sound
    private IEnumerator FadeOutSound(AudioSource source, float duration)
    {
        float currentTime = 0;
        float startVolume = source.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, currentTime / duration);
            yield return null;
        }
        source.volume = 0;
        source.Stop(); // Stop the sound after fading out
    }
}
