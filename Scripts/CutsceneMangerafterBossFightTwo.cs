using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneMangerafterBossFightTwo : MonoBehaviour
{
    public DialogueManager dialogue;
    public GameObject player;
    private string nextLevel = "Level_3";

    private playercontroller playerController;
    [SerializeField] private Animator playerAnimator;
    private Rigidbody2D rigid;
    private playeranimator pa;

    private bool cutsceneStarted = false;

    private void Start()
    {
        if (dialogue != null)
        {
            dialogue.gameObject.SetActive(false);
        }

        if (player != null)
        {
            playerController = player.GetComponent<playercontroller>();
            rigid = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponentInChildren<Animator>();
            pa = player.GetComponentInChildren<playeranimator>();
        }
    }

    public void StartCutScene()
    {
        if (cutsceneStarted) return;
        cutsceneStarted = true;

        if (playerController != null)
        {
            DisablePlayerMovement();
        }

        if (dialogue != null)
        {
            dialogue.gameObject.SetActive(true);
        }

        string[] dialogueLines = {
            "(Softly, with ancient calm)\n Ah... the weight returns heavier this time.",//aw
            "\nTell me, King's Champion.... what did you see?",//aw
            "(Low, his voice frayed with exhaustion)\n I fought myself.",//k
            "\nAnd... which one of you bled?",//aw
            "(Staring deeper into the flames)\n I don't know. Maybe both. Maybe neither. It didn't matter.",//k
            "\nI heard him... after the blade was buried in his chest. I still hear him.",//k
            "(The knight tightens his posture, the firelight casting long shadows across his armor.)\n I was supposed to feel somthing... relief, closure, peace.",//k
            "(Scoffs bitterly)\n But all I feel is the silence he left behind.",//k
            "(Poking at the fire with a crooked branch)\n And what did you bury, hmm? A ghost? A mistake? Or a truth too heavy to carry?",//aw
            "\nYou wear pain like armor, King's Champion. But even steel cracks under time and sorrow.",//aw
            "(Softer now)\n There are wounds you cannot fight. Only endure. And in enduring, understand.",//aw
            "\nI don't want to understand it. I just want it to end.",//k
            "(A beat, then a whisper)\n And if it ends, what will be left of you?",//aw
            "There is no shame in carrying grief. It means you still have something worth losing. \nBut beware the day you feel nothing. that is when the shadows win.",//aw
            "(Steps foward, voice like a hush of wind through dying leaves)\n Then rise. Carry your sarrow not as a burden... but as a blade.",//aw
            "\nThe raod ahead will not forgive you. So do not wait for it to.",//aw

        };

        string[] speaker = {
            "Astral Wizard",
            "Astral Wizard",
            "Knight",
            "Astral Wizard",
            "Knight",
            "Knight",
            "Knight",
            "Knight",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Knight",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
        };

        dialogue.StartDialogue(dialogueLines, speaker);

        StartCoroutine(WaitForDialougeToEnd());
    }

    private IEnumerator WaitForDialougeToEnd()
    {
        yield return new WaitUntil(() => dialogue.IsDialogueFinished());

        EnablePlayerMovement();


        Debug.Log("loading to the next level");

        SceneManager.LoadScene(nextLevel);

    }

    private void DisablePlayerMovement()
    {
        StartCoroutine(DisablePlayerMovementwithDelay());
    }

    private IEnumerator DisablePlayerMovementwithDelay()
    {
        if (pa != null)
        {
            pa.SetCutsceneMode(true);
        }
        Debug.Log("Set to idle animation");
        // Wait for a short delay 
        yield return new WaitForSeconds(0.1f);

        if (playerController != null)
        {
            playerController.enabled = false; // Disable player controller
            rigid.linearVelocity = Vector2.zero; // Stop movement
            rigid.constraints = RigidbodyConstraints2D.FreezeAll; // Prevent further movement
        }
    }

    public void EnablePlayerMovement()
    {
        if (pa != null)
        {
            pa.SetCutsceneMode(false);
        }
        if (playerController != null)
        {
            Debug.Log("Enabling player movement...");
            rigid.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation; // Restore normal physics
            playerController.enabled = true; // Enable player controller
        }
    }

}
