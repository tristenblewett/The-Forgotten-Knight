using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneMangerAfterLevelTwo : MonoBehaviour
{
    public DialogueManager dialogue;
    public GameObject player;
    

    private playercontroller playerController;
    [SerializeField] private Animator playerAnimator;
    private Rigidbody2D rigid;
    private playeranimator pa;
    [SerializeField] private ToBossTwoFightTransition bossTwo;

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
        "(Voice calm, words slow and deliberate)\n Ah... the wind carries the scent of steel and sorrow.",//wizard
         "(Pauses, stirring the fire with a gnarled staff)\n A warrior walks among the stars tonight... burdened, lost, but not without purpose.",//wizard
        "(Standing at the edge of the firelight, voice low, skeptical)\n I didn't come for riddles, old man.",//knight
        "(Chuckles softly, still not looking at the knight)\n And yet, here you stand.",//Wizard
        "(Glances up, his gaze piercing yet kind)\n Sit, knight. You have walked far enough to know that swords do not cut away ghosts.",//wizard
        "(Staring into the flames)\n Ghosts? That's all that's left of me.",//knight
        "(Nods, poking the fire gently)\n And so, you wear your past like a second skin... heavy, unyielding. But tell me, knight.",//Wizard
        "(Tilts his head)\n is it your past that haunts you, or the  man you were supposed to become?",//wizard
        "(scoffs, bitter)\n What does it matter? That man died on the battlefield long ago.",//knight
        "(A small, knowing smile)\n Die he? ",//wizard
        "(Taps his staff against the ground)\n Strange, then, that he sits before me now.",//wizard
        "(Voice tight, controlled)\n The man I was... he fought for something. He velieved. But belief doesn't stop betrayal. It doesn't stop time from swallowing you whole.",//knight
        "(Softly, looking up at the stars)\n No... but it keeps the fire lit, even in the coldest of nights.",//wizard
        "(Looking away, his voice quieter)\n What if I don't want it anymore? The fire. The fight.",//knight
        "(Chuckles, shaking his head)\n Ah... there it is. The weight in your voice, the burden in your step. You do not seek victory, nor even revenge... ",//wizard
        "(His gaze sharpens)\n You seek an ending.",//wizard
        "(Leaning forward, voice gentle yet firm)\n But endings, my friend, are not found at the edge of a blade. They are found in the choices we make when the battle is over.",//wizard
        "(His voice is almost a whisper)\n And if the battle never ends?",//knight
        "(Smiling faintly, his voice carrying a deep warmth)\n Then you must decide, are you the sword, forever seeking war... or the hand that chooses when to set it down?",//wizard
        "(Nods approvingly)\n Heavy is the armor that carries only regret. But even the heaviest burdens can be set aside... when the warrior chooses to live, rather than simply survive.",//wizard
        };

        string[] speaker = {
        "Astral Wizard",
        "Astral Wizard",
        "Knight",
        "Astral Wizard",
        "Astral Wizard",
        "Knight",
        "Astral Wizard",
        "Astral Wizard",
        "Knight",
        "Astral Wizard",
        "Astral Wizard",
        "Knight",
        "Astral Wizard",
        "Knight",
        "Astral Wizard",
        "Astral Wizard",
        "Astral Wizard",
        "Knight",
        "Astral Wizard",
        "Astral Wizard"
        };

        dialogue.StartDialogue(dialogueLines, speaker);
        StartCoroutine(WaitForDialougeToEnd());
    }

    private IEnumerator WaitForDialougeToEnd()
    {
        yield return new WaitUntil(() => dialogue.IsDialogueFinished());
        EnablePlayerMovement();
        bossTwo.FinishDialogue();

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
}
