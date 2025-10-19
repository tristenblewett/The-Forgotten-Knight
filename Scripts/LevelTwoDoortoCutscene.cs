using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTwoDoortoCutscene : Interactable
{
    public float doorTimer = 5f;
    private string nextSceneName = "cutsceneafterlevel_2";
    private bool isDoorActive = false;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorOpen;
    [SerializeField] private AudioClip doorClose;

    private void Awake()
    {
        gameObject.SetActive(false);
        Lever.OnLeverPulled += CheckLever;
    }

    private void OnDestroy()
    {
        Lever.OnLeverPulled -= CheckLever;
    }

    private void CheckLever(Lever lever)
    {
        if(LeverPuzzleManager.Instance.IsCorrectLever(lever))
        {
            ActivateDoor();
        }
    }

    private void ActivateDoor()
    {
        gameObject.SetActive(true);
        isDoorActive = true;

        if(audioSource != null && doorOpen != null)
        {
            audioSource.clip = doorOpen;
            audioSource.Play();
        }

        Debug.Log("Door Activated! Press 'E' to enter.");
        StartCoroutine(DeactivateDoorAfterDelay());
    }

    private IEnumerator DeactivateDoorAfterDelay()
    {
        yield return new WaitForSeconds(doorTimer);

        if(audioSource != null && doorClose != null)
        {
            audioSource.clip = doorClose;
            audioSource.Play();
        }

        isDoorActive = false;
        gameObject.SetActive(false);
        Debug.Log("Door deactivated!");
    }

    protected override void Interact()
    {
        if(isDoorActive)
        {
            Debug.Log("Player interacted with the door. Loading the next scene");
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("the door is locked. find the correct lever!");
        }
    }
}
