using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger Instance;

    private int collectedKeys = 0;
    public int requiredKeys = 3;

    public AudioClip keyCollectSound;
    public AudioClip allKeysCollectedSound;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void CollectKey()
    {
        collectedKeys++;

        if(keyCollectSound != null)
        {
            audioSource.PlayOneShot(keyCollectSound);
        }

        if(collectedKeys >= requiredKeys)
        {
            PlayAllKeysCollectedSound();
        }
    }

    private void PlayAllKeysCollectedSound()
    {
        if(allKeysCollectedSound != null)
        {
            audioSource.PlayOneShot(allKeysCollectedSound);
        }
    }

    public bool HasRequiredKeys()
    {
        return collectedKeys >= requiredKeys;
    }
}
