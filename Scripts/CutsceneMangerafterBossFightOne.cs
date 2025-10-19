using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneMangerafterBossFightOne : MonoBehaviour
{
    public DialogueManager dialogue;
    public GameObject player;
    [SerializeField] private CutsceneToLevelTwoDoor doorController;

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

        if(playerController != null)
        {
            DisablePlayerMovement();
        }

        if(dialogue != null)
        {
            dialogue.gameObject.SetActive(true);
        }

        string [] dialogueLines = { 
        "(Voice cold, his eyes glowing faintly red, barely concealed anger simmering beneath his words)\n So, it's you... Still crawling through these halls, I see.",//knight1
        "(Sneering, crossing their arms with a look of disdain)\n Well, well. The \"King's Champion\" returns from the dead, or so we thought. Did you finally decide to grace us with your presence, or is this just another of your little delusions of grandeur?",//court2
        "(Voice low and biting, eyes burning with resentment)\n Delusions of grandeur? (Pauses, voice dripping with contempt) I'm the one who fought and bled for this kingdom, while you all sat in your warm chambers, safe behind your lies.",//knight3
        "(Shrugging nonchalantly, clearly dismissive)\n Fought and bled, you say? (A mockery in their tone) and where did that get you? Cast aside like an old sword. Left to rot when you were no longer useful.",//court4
        "(Voice hard, almost predatory, his hands tightening around his sword hilt)\n You think you can mock me? You think I don't remember? I was left... not by demons, but by you....",//knight5
        "(His voice drops to a growl)\n You and the rest of the court.",//knight6
        "(Sneering, taking a step forward with a mocking tone)\n Left to die, right? You're still bitter over that, I see. I wonder... did it take all these years to scrape together the will to come crawling back? How touching.",//court7
        "(Voice biting, full of suppressed rage)\n You call it crawling back? (He takes a slow, deliberate step closer, his eyes glowing even more intensely) I never left, and I'm not here for you or the court. I'm here becuase of her. The princess. She's the only one who remembers.",//knight8
        "(He reaches into his armor, his movements deliberate, and pulls out the letter she sent him. The edges are frayed, the paper worn, the ink slightly faded from the years. He holds it in his gloved hands, but his gaze softens as he looks at it, his emotions hidden beneath a mask of bitterness.)",//knight9
        "(Speaking through clenched teeth, his voice thick with bitterness)\n She sent for me. She's the only one who didn't forget. You all left me to die, and now, I'm here to fix the mess you made. (His gaze sharpens, a quiet intensity in his eyes, his grip tightening around the letter as he crumples it slightly) I'll save her, even if none of you deserve it." +//knight10
        "\n(He folds the letter back into his armor, the anger and pain quickly returning to his face. His eyes now burn with a deep, quiet resolve as he stares at the court member.)",
        "(Raising an eyebrow, sneering)\n The princess sent for you? How quaint. I hadn't heard that. Seems a little too convenient, doesn't it? (A sharp, cynical laugh) Maybe she just doesn't know what you really are anymore.\n" +
        "(The knight's eyes flare red, and for a brief moment, it seems like his restraint will snap.)",//court
        "(Voice low, barely controlled)\n What I am... is the last person who will save this kingdom. You didn't care when they abandoned me. When you left me for dead in the final battle of the Great Crusade.\n" +
        "(He steps closer, voice lowering to a cold, dangerous whisper, every word dripping with veom and supressed emotion.)",//knight
        "(Voice hollow, like the weight of his past is crushing him)\n They cast me aside after we defeated the Chaos Demon horde. Left me there on that battlefield with no chance to breathe, to recover. The princess..(His voice tightens as he catches himself, focusing on the court member again) She didn't forget. She knew the truth. The rest of you? You betrayed me.",//knight
        "(Voice faltering for a brief moment, clearly caught off guard)\n Wait.. You're telling me...(Pauses, disbelief in thier voice) The princess was... taken?",//court
        "(Leaning in, eye glowing a fierce red now, his voice full of quiet rage)\n By the Demon Prince. He took her. Right in front of me. The last time I saw her, she was in his grasp.\n" +
        "(He pulls back, the anger and frustration boiling over, but he quickly reins it in, his tone cold once again as he turns to walk away.)",//knight
        "(Voice dangerously calm, almost a whisper)\n You don't know her. You don't know what she meant to me.. what she still means. I'll save her. Even if it's the last thing I do.\n" +
        "(As he walks away, the court member stands in stunned silence, trying to process the shocking revelation.)"//knight
        };

        string[] speaker = { 
        "Knight",//1
        "Court Member",//2
        "Knight",//3
        "Court Member",//4
        "Knight",//5
        "Knight",//6
        "Court Member",//7
        "Knight",//8
        "Knight",
        "Kinght",
        "Court Member",
        "Knight",
        "Knight",
        "Court Member",
        "Knight",
        "Knight"
        };

        dialogue.StartDialogue(dialogueLines, speaker);

        StartCoroutine(WaitForDialougeToEnd());
    }

    private IEnumerator WaitForDialougeToEnd()
    {
        yield return new WaitUntil(() => dialogue.IsDialogueFinished());

        doorController.EnableInteraction();
        Debug.Log("door interaction enabled.");
        EnablePlayerMovement();

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
