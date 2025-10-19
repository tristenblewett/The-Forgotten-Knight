using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelThreeDoorToCutscene : Interactable
{
    private bool isUnlocked = false;
    private string sceneToLoad = "cutsceneafterlevel_3"; 

    public void Unlock()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            Debug.Log("Door unlocked!");

        }
    }

    protected override void Interact()
    {
        if (isUnlocked)
        {
            Debug.Log("Interacting with unlocked door. Loading scene...");
            LoadNextScene();
        }
        else
        {
            Debug.Log("Door is locked. Cannot proceed.");
        }
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene to load is not set on the Door!");
        }
    }
}
