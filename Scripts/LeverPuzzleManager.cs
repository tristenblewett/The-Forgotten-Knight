using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverPuzzleManager : MonoBehaviour
{
    public static LeverPuzzleManager Instance;
    private Lever correctLever;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctLeverSound;
    [SerializeField] private AudioClip wrongLeverSound;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        AssignRandomLever();
    }

    private void AssignRandomLever()
    {
        Lever[] levers = FindObjectsOfType<Lever>();
        if(levers.Length > 0)
        {
            correctLever = levers[Random.Range(0, levers.Length)];
            Debug.Log("Correct Lever:" + correctLever.gameObject.name);
        }
    }

    public void OnLeverPulled(Lever lever)
    {
        if(lever == correctLever)
        {
            PlaySound(correctLeverSound);
            FinalPuzzleManager.Instance.MarkObjectiveComplete("Lever");
            Debug.Log("Correct lever pulled!");
        }
        else
        {
            PlaySound(wrongLeverSound);
            Debug.Log("Wrong Lever pulled.");
        }
    }

    public bool IsCorrectLever(Lever lever)
    {
        return lever == correctLever;
    }

    private void PlaySound(AudioClip clip)
    {
        if(audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
