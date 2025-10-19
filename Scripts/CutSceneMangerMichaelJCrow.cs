using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneMangerMichaelJCrow : MonoBehaviour
{
    public DialogueManager dialogueManager; // DialogueManager reference

    public GameObject player;
    [SerializeField] private MichaelJCrow mjc;

    private string nextLevel = "cutsceneafterbossfight_3";

    private playercontroller playerController;
    [SerializeField] private combo playerCombo;
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
            playerCombo = player.GetComponent<combo>();
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

        if (mjc != null)
        {
            mjc.enabled = false;//disable the script
            mjc.fightStarted = true;
        }

        // dialogue
        string[] dialogueLines = {
            " (Smiling lazily, voice calm but laced with venom)\n There you are. The ghost in rusted armor. The last scrap of a dying story.",//mjc1
            " (Stepping forward slowly, eyes glowing red beneath the helm)\n And you're the carrion bird that picks at the corpse of kingdoms.",//k1
            " (Chuckling, arms spread in mock welcome)\n Oh, I do far more tha pick. I build someting new from the bones. Something honest.\n No lies of glory, no thrones built on the backs of broken men like you",//mjc2
            " (Voice steady, cold)\n You think destruction is truth? That suffering gives you meaning?",//k2
            " (Leaning in slightly, eye narrowing)\n I think truth hurts. And I embrace it.",//mjc3
            "\n Unlike youre precious princess, hiding behind hope like it's armor.",//mjc4
            " (Soft chuckle)\n She believed you were more than the grave they shoved you into.",//mjc5
            "\n Admirable. Naive.",//mjc16
            " (Quietly, after a pause)\n She remembered. When no one else did.",//k3
            " (Smiling wider, like he's found a crack in the armor)\n Ah. There it is. Still carrying her voice like a blade in youre chest.",//mjc6
            "\n But tell me.... What does she become when hope breaks?", //mjc7
            "\n What's left of a light when the darkness finally reaches it?",//mjc8
            " (Voice low, laced with suppressed anger)\n Not yours to decide.",//k4
            " (Stepping forward, softly, like a whisper)\n No... but it will be mine to watch. When she calls out in her last breath... do you think she'll say your name?",//mjc9
            "\n Or will she curse it, like the rest of them?",//mjc10
            " (A beat of silence. His hand tightens on his sword hilt.)\n Careful what name you speak, Crow. Some still carry weight. Even in the dark.",//k5
            " (Scoffing)\n You walk like a man with purpose, but I see what's under the armor.",//mjc11
            "\n It's not love. It's not duty. It's what's left when the world has bled you dry.",//mjc12
            " (Pauses)\n You're not fighting to save her. You're fighting because without her... you're already gone.",//mjc13
            " (Voice like gravel, heavy with silence)\n Maybe. But even ghosts can kill.",//k6
            " (Spreads his arm, inviting)\n Then come, shadow. Show me what lingers after everything else dies.",//mjc14
            " (Grinning)\n Let's see what dies first, Knight... youre body, or the last piece of her you still carry.",//mjc15
            
        };

        string[] speakers = {
           "Michael J Crow",
           "Knight",
           "Michael J Crow",
           "Knight",
           "Michael J Crow",
           "Michael J Crow",
           "Michael J Crow",
           "Michael J Crow",
           "Knight",
           "Michael J Crow",
           "Michael J Crow",
           "Michael J Crow",
           "Knight",
           "Michael J Crow",
           "Michael J Crow",
           "Knight",
           "Michael J Crow",
           "Michael J Crow",
           "Michael J Crow",
           "Knight",
           "Michael J Crow",
           "Michael J Crow",


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

        if(playerCombo != null)
        {
            playerCombo.enabled = false;
        }
    }

    private IEnumerator EnablePlayerAndStartFight()
    {
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(false);
        }

        EnablePlayerMovement();

        yield return new WaitForSeconds(1.5f);

        if (mjc != null)
        {
            mjc.enabled = true; //enables the script
            mjc.fightStarted = false;
            mjc.StartBossFight();
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

        if (playerCombo != null)
        {
            playerCombo.enabled = true;
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
            " (Choking on blood, a crooked grin forming)\n Heh... you're still standing. Guess rust holds better than i thought.",//mjc1
            " (Voice hoarse, low)\n I didn't come here to stand.",//k1
            " (Laughs weakly)\n Right... you came to save her.",//mjc2
            "\n Still chasing light in a kindom that forgot what warmth is.",//mjc3
            " (Steps closer, eyes glowing faintly red)\n She remembered.",//k2
            " (A flicker of something almost like respect)\n Then she'll be the last",//mjc4
            "\n And you... you'll keep walking.",//mjc5
            "\n Until there's nothing left of you but steel and silence.",//mjc6
            " (Quietly)\n There's nothing left now.",//k3
            " (Laughs, then coughs violently, the sound sharp and wet)\n Hah... someone has to fill the silence. You wear it like a crown.",//mjc7
            " (Pauses, eyes flicking up)\n Tell me... did you feel it? Even now, that emptiness where your heart should be?",//mjc8
            " (Flat, almost numb)\n It's always been there.",//k4
            " (Smirking faintly)\n Then maybe we're not so different after all.",//mjc9
            " (He exhales slowly, words slipping through blood and breath)\n But you... you still think you can save her, don't you?",//mjc10
            " (Stepping closer, voice like ash)\n This was never about saving.",//k5
            " (Chuckles fainly, voice beginning to fade)\n Ah... then you really are lost.",//mjc11
            " (Beat)\n She'll die, you know. They all do.",//mjc12
            "\n Hope always rots before the flesh.",//mjc13
            " (Voice low, final)\n Then I'll bury the world with her.",//k6
            " (A dying smile, the last flicker of defiance in his eyes)\n Good...",//mjc14
            "\n Be the Monster they feared...",//mjc15
            "\n You wear it well....",//mjc16
        };

        string[] speakers = {
            "Michael J Crow",
            "Knight",
            "Michael J Crow",
            "Michael J Crow",
            "Knight",
            "Michael J Crow",
            "Michael J Crow",
            "Michael J Crow",
            "Knight",
            "Michael J Crow",
            "Michael J Crow",
            "Knight",
            "Michael J Crow",
            "Michael J Crow",
            "Knight",
            "Michael J Crow",
            "Michael J Crow",
            "Michael J Crow",
            "Knight",
            "Michael J Crow",
            "Michael J Crow",
            "Michael J Crow",
        };

        dialogueManager.StartDialogue(postFightDialogue, speakers);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        if(mjc != null)
        {
            mjc.Die();
        }

        EnablePlayerMovement();

        dialogueManager.gameObject.SetActive(false);

        StartCoroutine(NextScene());
    }

    private IEnumerator NextScene()
    {
        yield return new WaitForSeconds(4.5f);

        Debug.Log("loading to the next level");

        SceneManager.LoadScene(nextLevel);
    }
}
