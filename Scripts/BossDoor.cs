using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDoor : Interactable
{
    private bool canInteract = false;
    private string NextScene = "cutsceneafterbossfight_1";

    [SerializeField] private AudioSource teleportSource;
    [SerializeField] private AudioClip teleportSound;

    private float timeBeforeTP = 2.5f;

    // Enable interaction with the door
    public void EnableInteraction()
    {
        canInteract = true;
        Debug.Log("The door is now interactable!");
        StartCoroutine(EnterBossArena());
    }

    // Override the Interact method from the base Interactable class
    protected override void Interact()
    {
        Debug.Log("Interacting with the door. Transitioning to boss level...");
        //EnterBossArena();
    }

    private IEnumerator EnterBossArena()
    {
        yield return new WaitForSeconds(timeBeforeTP);

        if (!string.IsNullOrEmpty(NextScene))
        {
            Debug.Log($"Loading scene: {NextScene}");
            SceneManager.LoadScene(NextScene);
            teleportSource.PlayOneShot(teleportSound);
        }
        else
        {
            Debug.LogWarning("Boss scene name is not set!");
        }
    }
}

