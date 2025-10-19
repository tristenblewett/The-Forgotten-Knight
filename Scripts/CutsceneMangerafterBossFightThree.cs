using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneMangerafterBossFightThree : MonoBehaviour
{
    public DialogueManager dialogue;
    public GameObject player;
    private string nextLevel = "Level_4";

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
            "(Softly, watching the sky through the window)\n So... the shadow falls behind you now.",//aw1
            "(Low, exhausted)\n He said I was already empty. That I wear silence like a crown.",//k1
            "(Chuckles gently, not unkind)\n Hmph. Empty? No.",//aw2
            "\n Wounded, yes. Heavy with loss, yes. But empty....?",//aw3
            "\n No. Even silence has weight.",//aw4
            "(Slowly standing, staff in hand)\n I have seen men fall for less than what you carry.",//aw5
            "\n Seen strong hearts turned to stone.",//aw6
            "\n But not you. You still walk. Even when the ground bleeds beneath you feet.",//aw7
            "(Quiet)\n I don't know if I'm walking toward anything... or just away.",//k2
            "\n Does it matter? The path is made in the walking.",//aw8
            "(He steps closer, voice deeper now)\n You are not here because you are whole.\n You are here because you endure.",//aw9
            "\n Becuase every scar, every crack in that old armor, is proof that you chose to rise again.",//aw10
            "(Placing a hand over his heart)\n They will not remember the war. Nor the blood.",//aw11
            "\n But they may remember the man who stood when no one else would.",//aw12
            "(Speaking bitterly)\n I'm not a hero. I never was.",//k3
            "(Leaning in, eyes gleaming with quiet fire)\n No. You're something rarer.",//aw13
            "\n A man who kept walking after the story ended.",//aw14
            "\n A man who looked into the dark and did not ask to be saved.",//aw15
            "\n Beyond the bridge lies the marrow of evil. The place where names are forgotten.",//aw16
            "\n It will try to unmake you. It will speak in voices you know.",//aw17
            "\n But remember this....",//aw18
            "\n You are not just the sword.",//aw19
            "\n You are the hand that held it. The will behind it.",//aw20
            "\n And no darkness... not even death... can take that from you.",//aw21
            "(Softly)\n This is the end, then.",//k4
            "(Shaking his head gently)\n No... this is the last truth.",//aw22
            "\n And whether you live... or vanish into legend...",//aw23
            "\n Make sure they remember that you were real.",//aw24
        };

        string[] speaker = {
            "Astral Wizard",
            "Knight",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Knight",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Knight",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Astral Wizard",
            "Knight",
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

        StartCoroutine(NextScene());

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

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(3f);

        Debug.Log("loading to the next level");

        SceneManager.LoadScene(nextLevel);
    }
}
