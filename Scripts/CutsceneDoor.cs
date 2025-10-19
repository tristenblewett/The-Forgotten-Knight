using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneDoor : Interactable
{
    private bool canInteract = false;
    private string bossScene = "1stBoss";

    // Enable interaction with the door
    public void EnableInteraction()
    {
        canInteract = true;
        Debug.Log("The door is now interactable!");
    }

    // Override the Interact method from the base Interactable class
    protected override void Interact()
    {
        Debug.Log("Interacting with the door. Transitioning to boss level...");
        EnterBossArena();
    }

    private void EnterBossArena()
    {
        if (!string.IsNullOrEmpty(bossScene))
        {
            Debug.Log($"Loading scene: {bossScene}");
            SceneManager.LoadScene(bossScene);
        }
        else
        {
            Debug.LogWarning("Boss scene name is not set!");
        }
    }
}
