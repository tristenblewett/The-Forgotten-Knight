using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : Interactable
{
    public AudioClip doorUnlockSound;
   [SerializeField] private AudioSource audioSource;

    private string nextSceneName = "cutsceneafterlevel_1";

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected override void Interact()
    {
        if(GameManger.Instance.HasRequiredKeys())
        {
            UnlockDoor();
        }
        
    }

    private void UnlockDoor()
    {
        if(doorUnlockSound != null)
        {
            audioSource.PlayOneShot(doorUnlockSound);
        }

        if(!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
