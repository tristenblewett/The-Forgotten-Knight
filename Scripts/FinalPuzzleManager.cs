using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPuzzleManager : MonoBehaviour
{
    public static FinalPuzzleManager Instance;

    [SerializeField] private GameObject finalDoor;

    [SerializeField] private bool useTimer = true;
    [SerializeField] private float timeLimit = 300f;
    private float currentTime;

    private bool keyDone = false;
    private bool leverDone = false;
    private bool skullDone = false;

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
    }

    private void Start()
    {
        if(useTimer)
        {
            currentTime = timeLimit;
        }
    }

    private void Update()
    {
        if(useTimer && !IsComplete())
        {
            currentTime -= Time.deltaTime;
            if(currentTime <= 0f)
            {
                Debug.Log("Time is up! Puzzle reset");
                ResetPuzzle();
            }
        }
    }

    public void MarkObjectiveComplete(string objectiveName)
    {
        switch(objectiveName)
        {
            case "Key": keyDone = true; Debug.Log("Key Completed"); break;
            case "Lever": leverDone = true; Debug.Log("Lever COmpleted"); break;
            case "Skulls": skullDone = true; Debug.Log("Skulls Completed"); break;
        }

        if(IsComplete())
        {
            UnlockFinalDoor();
        }
    }

    private bool IsComplete()
    {
        return keyDone && leverDone && skullDone;
    }

    public void UnlockFinalDoor()
    {
        if(IsComplete())
        {
            FinalDoor door = FindAnyObjectByType<FinalDoor>();
            if(door != null)
            {
                door.SetUnlocked(true);
            }
        }
    }

    public void ResetPuzzle()
    {
        keyDone = false;
        leverDone = false;
        skullDone = false;

        SkullLamp.litCount = 0;

        Lever[] levers = FindObjectsOfType<Lever>();
        foreach (Lever lever in levers)
        {
            lever.SendMessage("ResetLever", SendMessageOptions.DontRequireReceiver);
        }

        if (useTimer)
        {
            currentTime = timeLimit;
        }
    }
}
