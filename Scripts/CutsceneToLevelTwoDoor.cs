using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneToLevelTwoDoor : Interactable
{
    private bool canInteract = false;
    private string leveltwoscene = "Level_2";

    public void EnableInteraction()
    {
        canInteract = true;
        Debug.Log("Door can now be interacted with.");
    }

    protected override void Interact()
    {
        Debug.Log("Interacting with the door. Transitioning to level two...");
        EnterLevelTwo();
    }

    private void EnterLevelTwo()
    {
        if (!string.IsNullOrEmpty(leveltwoscene))
        {
            Debug.Log($"Loading scene: {leveltwoscene}");
            SceneManager.LoadScene(leveltwoscene);
        }
        else
        {
            Debug.LogWarning("Level Two scene name is not set!");
        }
    }
}
