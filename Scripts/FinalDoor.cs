using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoor : Interactable
{
    public AudioClip doorUnlockSound;
    [SerializeField] private AudioSource audioSource;

    private bool isUnlocked = false;

    private string nextSceneName = "";


    protected override void Interact()
    {
        if (isUnlocked)
        {
            UnlockDoor();
        }
        else
        {
            Debug.Log("The final door is locked");
        }
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        if(isUnlocked)
        {
            Debug.Log("Final Door is Unlocked");
        }
    }

    private void UnlockDoor()
    {
        if (doorUnlockSound != null)
        {
            audioSource.PlayOneShot(doorUnlockSound);
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
