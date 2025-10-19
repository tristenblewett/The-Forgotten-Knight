using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkullLamp : Interactable
{
    [SerializeField] private Light2D lampLight; // Drag your Light2D component here
    [SerializeField] private AudioSource lightSound; // Optional: sound when lighting

    private bool isLit = false;

    public static int litCount = 0;
    public static int totalToUnlock = 5;

    [SerializeField] private LevelThreeDoorToCutscene doorToUnlock;

    private void Start()
    {
        if (lampLight != null)
        {
            lampLight.enabled = false; // Start with the lamp turned off
        }
    }

    protected override void Interact()
    {
        if (!isLit)
        {
            isLit = true;
            litCount++;

            Debug.Log("Skull Lamp lit! Total lit: " + litCount);

            if (lampLight != null)
            {
                lampLight.enabled = true; // Turn on the Light2D
            }

            if (lightSound != null)
            {
                lightSound.Play(); // Play light-up sound
            }

            if (litCount >= totalToUnlock)
            {
                if (doorToUnlock != null)
                {
                    doorToUnlock.Unlock();
                }
                FinalPuzzleManager.Instance.MarkObjectiveComplete("Skulls");//final puzzle
            }

            StartCoroutine(FadeInLight());
        }
        else
        {
            Debug.Log("Lamp already lit.");
        }
    }

    private IEnumerator FadeInLight()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float startIntensity = 0f;
        float targetIntensity = 1f; // Or whatever you set your lamp normally

        lampLight.intensity = startIntensity;
        lampLight.enabled = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            lampLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }

        lampLight.intensity = targetIntensity;
    }
}
