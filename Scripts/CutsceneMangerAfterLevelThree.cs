using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneMangerAfterLevelThree : MonoBehaviour
{
    public DialogueManager dialogue;
    public GameObject player;
    private string nextLevel = "3rdBoss";

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
            "(Stumbling out from behind a pillar, cloak torn, face straked with soot and panic)\n Y-You! What are you doing here? You... you should be dead!",//cm
            "(Quietly, voice hollow, his eyes glowing faintly red)\n I was.",//k
            "(Backing up, clearing shaken, still trying to posture)\n This... this is madness. The outer walls have fallen.\n The demons.... they're inside the throne wing.\n Where are the king's guards? where is the command?!",//cm
            "(Stepping closer, voice like ice scraping metal)\n Gone. Just like your courage.",//k
            "(Panicked)\n You think this is my fault?! I didn't open the gates! I didn't call the darkness here!",//cm
            "(Still advancing, a quiet rage growing)\n No. You only fed it. With silence. With cowardice. With betrayal.",//k
            "(Angrily, desperate to deflect blame)\n We did what we had to! The court had to protect what was left!\n You.... you were the past. A failed weapon. A name cursed in whispers!",//cm
            "(Stops. His hand flexes at his side. His voice lowers, bitter and dry.)\n And now the past walks your halls again, dressed in blood and smoke.\n I hope it haunts you.",//k
            "(Hesitates, then spits out)\n You think the princess will save you from what you are?\n That letter, her pity, makes you whole again?",//cm
            "(A pause. He says nothing. But his hand slips briefly inside his armor. Touches something folded there. The edge of an old, dirt smudged letter.)\n She Believed in Somthing worth saving. Not the kingdom.\n Not the throne. Just... the memory of who i was. ",//k
            "(Voice Cracking, sweat dripping as the castle trembles again)\n They're inside the trone wing....! The gates didnt hold.... gods help us, we're going to be torn apart!",//cm
            "(Calm, cold, almost indifferent)\n You built a palace of marble on a foundation of bones. It was always going to crumble.",//k
            "(Stumbling back, fear overtaking pride)\n This is not how it was meant to be!\n The kingdom was supposed to endure! We held it together! You.... if you'd just stayed gone!",//cm
            "(Stepping forward slowly, voice low and cruel)\n You mean if I'd stayed buried. Forgotten. Convenient.",//k
            "(Backing up, voice trembling)\n You're not the same. There's something wrong with you.. Your eyes... Your voice... What did you bring back with you?",//cm
            "(Looking down slightly, pausing)\n Silence. Regret. The taste of rust in my mouth.",//k
            "(Calling after him, voice trembling now... not with anger, but fear)\n What are you?",//cm
            "(Doesn't stop walking. But his voice carries like a shadow.)\n What you made me...."//k
        };

        string[] speaker = {
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
            "Court Member",
            "Knight",
        };

        dialogue.StartDialogue(dialogueLines, speaker);

        StartCoroutine(WaitForDialougeToEnd());
    }

    private IEnumerator WaitForDialougeToEnd()
    {
        yield return new WaitUntil(() => dialogue.IsDialogueFinished());

        EnablePlayerMovement();
        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(4f);
        
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
