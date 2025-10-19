using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneMangerPastKnightBoss : MonoBehaviour
{
    public DialogueManager dialogueManager; // DialogueManager reference
    
    public GameObject player;
    [SerializeField] private PastKnightBoss bossScript;

    private string nextLevel = "cutsceneafterbossfight_2";

    private playercontroller playerController;
    private Rigidbody2D rigid;
    [SerializeField] private Animator playerAnimator;
    private playeranimator pa;

    private bool cutsceneStarted = false;

    private void Start()
    {
       

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(false);
        }

        if (player != null)
        {
            playerController = player.GetComponent<playercontroller>();
            rigid = player.GetComponent<Rigidbody2D>();
            playerAnimator = player.GetComponentInChildren<Animator>();
            pa = player.GetComponentInChildren<playeranimator>();
        }
    }

    public void StartCutscene()
    {
        if (cutsceneStarted) return; // Prevent retriggering
        cutsceneStarted = true; 
        
        DisablePlayerMovement();

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(true);
        }

        if(bossScript != null)
        {
            bossScript.enabled = false;//disable the script
            bossScript.fightStarted = true;
        }

        // dialogue
        string[] dialogueLines = {
            "(Voice Cold, steady, yet filed with something deep...accusation? Pity?)\n Look at you.",//pk
            "(Tilts head slightly)\n A ghost wearing my skin. A blade dulled by time.",//pk
            "(Tenses, his voice low)\n I don't have time for illusions.",//k
            "(A mirthless chuckle)\n Illusion? No, I am moreee real than you. I am the warrior who did not falter. \nThe knight who did not hesitate.",//pk
            "(Takes a slow step forward)\n I am the man you were supposed to be.",//pk
            "(Tightening his grip on his sword)\n You died on the battlefield.",//k
            "(Smirks, raising his blade slightly)\n No, you died. You, who crawled from the dirt, abondoned, broken, forsaken. You died the moment you stopped believing in the fight. The moment you let hate and loneliness take hold.",//pk
            "(Pauses)\n Tell me, what do you fight for now? Honor? Redemption?",//pk
            "(The Knight remains silent, but his hand clenches. The past Knight watches him carefully.)",//k
            "Ah.... it's her, isn't it?",//pk
            "(A dark chuckle)\n The last flicker of warmth in a dying fire",//pk
            "(lowering his voice)\n But fires burn out, don't they?",//pk
            "(Eyes narrowing, his breath uneasy)\n What are you saying?",//k
            "(Smiling, but there is something cruel behind it)\n The princess. You already failed her once. And now....",//pk
            "(No longer composed, his voice laced with anger, denial)\n LIES!!!",//k
            "(Soft chuckle, lifting his blade)\n Then prove it.",//pk
        };

        string[] speakers = {
           "Past Knight",
           "Past Knight",
           "Knight",
           "Past Knight",
           "Past Knight",
           "Knight",
           "Past Knight",
           "Past Knight",
           "Knight",
           "Past Knight",
           "Past Knight",
           "Past Knight",
           "Knight",
           "Past Knight",
           "Knight",
           "Past Knight",


        };

        
        dialogueManager.StartDialogue(dialogueLines, speakers);

        StartCoroutine(EnablePlayerAndStartFight());
    }

    private void DisablePlayerMovement()
    {
        StartCoroutine(DisablePlayerMovementWithDelay());
    }

    private IEnumerator DisablePlayerMovementWithDelay()
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

    private IEnumerator EnablePlayerAndStartFight()
    {
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());
        
        if(dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(false);
        }

        EnablePlayerMovement();

        yield return new WaitForSeconds(1.5f);

        if (bossScript != null)
        {
            bossScript.enabled = true; //enables the script
            bossScript.fightStarted = false;
            bossScript.StartBossFight();
        }
    }
    private void EnablePlayerMovement()
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

    public void StartBossDefeatedSequence()
    {
        StartCoroutine(BossDefeatedSequence());
    }

    private IEnumerator BossDefeatedSequence()
    {
        yield return new WaitForSeconds(2f);

        DisablePlayerMovement();

        dialogueManager.gameObject.SetActive(true);

        string[] postFightDialogue = {
            "\nYou won. But tell me... what did you really kill?",//pk
            "\nDo you feel it? That emptiness? The Silence where something used to be?",//pk
            "(A low chuckle, distant yet suffocating)\n You thought this would bring you peace. That cutting me down would bury the past.",//pk
            "But the past doesn't die. It lingers. It festers.\n And when you close your eyes, when you think you are free-",//pk
            "(A pause, the air thick with unseen weight)\n I will still be there. ",//pk
            "\nLook at you. Still running. Still pretending this path leads anywhere but the same ending.",//pk
            "(The voice lowers, becoming something almost like a whisper, almost like his own voice)\n You know how this ends. You saw it, didn't you? Just before our blades met.",//pk
            "\nThe fire goes out. And you are left in the dark, alone...",//pk
        };

        string[] speakers = { 
        "Past Knight",
        "Past Knight",
        "Past Knight",
        "Past Knight",
        "Past Knight",
        "Past Knight",
        "Past Knight",
        "Past Knight",
        };

        dialogueManager.StartDialogue(postFightDialogue, speakers);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        EnablePlayerMovement();

        dialogueManager.gameObject.SetActive(false);

        Debug.Log("loading to the next level");

        SceneManager.LoadScene(nextLevel);
    }
}
