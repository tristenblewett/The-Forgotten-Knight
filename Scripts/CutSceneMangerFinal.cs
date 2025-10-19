using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneMangerFinal : MonoBehaviour
{
    public DialogueManager dialogueManager; // DialogueManager reference

    public GameObject player;
    [SerializeField] private FinalBoss finalBoss;
    [SerializeField] private playerHealth playerhealth;
    [SerializeField] private SpriteRenderer spriteRender;

    private string nextLevel = "credits";

    private playercontroller playerController;
    private Rigidbody2D rigid;
    [SerializeField] private Animator playerAnimator;
    private playeranimator pa;

    private bool cutsceneStarted = false;
    [SerializeField] private float dkmoveSpeed = 3f;
    [SerializeField] private float kmoveSpeed = 10f;
    [SerializeField] private float pmoveSpeed = 8f;
    [SerializeField] private Transform kingPoint;
    [SerializeField] private Transform knightPoint;
    [SerializeField] private Transform bossSprite;
    [SerializeField] private Transform playerSprite;
    [SerializeField] private Transform princess;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastOriginHeight = 1f; // how far above her to cast from
    [SerializeField] private float raycastLength = 3f; // length downwards
    [SerializeField] private float footOffset = 0.5f;

    [SerializeField] private Camera mainCam;
    [SerializeField] private Camera endingCam;
    private CameraFader mainFade;
    private CameraFader endingFade;

    private void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();

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

        mainFade = mainCam.GetComponent<CameraFader>();
        endingFade = endingCam.GetComponent<CameraFader>();

        mainCam = Camera.main;
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

        // dialogue
        string[] dialogueLines = {
            "(Quietly, studing the Knight)\n You have walked the marrow of the abyss... and yet here you stand.",//aw
            "Most men would have broken long before the stairs.",//aw
            "(Low, steady, but heavy with fatigue)\n I did break. I've just learned how to keep moving after.",//k
            "(Nods, almost solemnly approving)\n Ah... the lesson only suffering teaches. To wear the fracture as a shield.",//aw
            "Beyond those steps, the Demon King awaits. Not merely a beast of flesh and fire... but a mirror. He will hold every wound, every doubt, every hunger you carry.\n He will use them, twist them. that is his weapon.",//aw
            "(Tightening his grip on the sword hilt)\n Then I'll break that weapon. Even if it breaks me with it.",//k
            "(Shakes his head faintly, voice soft but sharp)\n No! This is not a battle of steel. Not truly.",//aw
            "Your blade will strike, yes... but it is your heart that will be weighed.",//aw
            "(Quietly)\n My heart... is ash.",//k
            "(His tone shifting, almost fierce)\n No. Ash is proof that fire once burned. And fire.. can be lit again.",//aw
            "Do not mistake ruin for emptiness.",//aw
            "(Softer now, almost a whisper)\n She remembered you, knight. When no one else did.",//aw
            "Carry that into the throne room. Not as a burden... but as a truth.",//aw
            "That even in a world of betrayal, there was one light that did not turn away.",//aw
            "Climb, knight! And let the end remember your name!",//aw
        };

        string[] speakers = {
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
           "Astral Wizard",
           "Astral Wizard",
           "Astral Wizard",
           "Astral Wizard",

        };


        dialogueManager.StartDialogue(dialogueLines, speakers);
        StartCoroutine(EnablePlayer());
    }
    //doesnt start the cutscene but the trigger is activated
    public void StartCutsceneWithBoss()
    {
        if (cutsceneStarted) return; // Prevent retriggering
        cutsceneStarted = true;

        DisablePlayerMovement();

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(true);
        }

        if (finalBoss != null)
        {
            finalBoss.enabled = false;//disable the script
            finalBoss.fightStarted = true;
        }
        
        // dialogue
        string[] dialogueLineswithBoss = {
            "(Voice rich, mocking, like a bell tolling for someone else's funeral)\n Well, well, well... the kingdom's ghost finally drags himself up the stairs. \nI could smell your sorrow from the rafters. How quaint.",//dk1
            "(Flat, controlled)\n I didn't come to be admired.",//k1
            "(Grinning, amusement dripping venom)\n No... you came to play at heroics. To clutch a memory and call it a cause.\n Tell me, relic... does the princess's little letter taste of victory? or only of dust?",//dk2
            "(Quiet, eyes hard)\n She remembered.",//k2
            "(Laughing, sharp)\n Remembered. How touching. A single candle flickering in a hourse of ash. You still cling to warmth where there is none. Pathetic and predictable.",//dk3
            "(Chuckling)\n Mock all you want.",//k3
            "(Leans forward voice low and crystalline with contempt)\n Do so, indeed. Mockery is my favorite course. You parade your pain like armor and call it courage.\n " +
            "I wear despair like a crown and it suits me. Tell me, when your last ember dies, will you scream her name or curse the fools who forgot you?",//dk4
            "(hardness behind the word)\n Neither.",//k4
            "(Laughs, the sound shaking the chamber)\n Do you not see? You and I are bound by the same chains.\n Abandoned. Betrayed. Left to rot while the world above pretends to sing!",//dk5
            "(Glowing eyes narrowing beneath the helm)\n You are nothing like me.",//k5
            "Nothing like you? Hahaha, I am you, knight the truth you bury under rust and silence. The hate you cradle when no one watches. The monster they already named you.",//dk6
            "(Hand tightening on his sword)\n  I was left to die, but I did not surrender. Like you said you wear despair like a crown. I carry it as a weapon.",//k6
            "(His voice thick with cruel delight)\n And yet, what will your weapon win you? The princess? She will wither like all flesh.\n The kingdom? It already lies in ash. You fight for nothing but ghosts.",//dk7
            "(Quiet, almost a whisper, but unshaken)\n Then I'll bury you with them. Finish this once and for all.",//k7
            "(Snarling with delighted cruelty)\n Finish, yes. Finish the joke. End the fairy tale. Let us see if your blade can cut through the truth. That everything you fight for is already dead.\n " +
            "Come then... show me the last thing you'll ever call noble",//dk8
            "(Staring at the Knight, voice booming with cruel anticipation)\n Very well. Dance, ghost. I will enjoy unmaking you.",//dk9
        };

        string[] speakers = {
           "Demon King",
           "Knight",
           "Demon King",
           "Knight",
           "Demon King",
           "Knight",
           "Demon King",
           "Knight",
           "Demon King",
           "Knight",
           "Demon King",
           "Knight",
           "Demon King",
           "Knight",
           "Demon King",
           "Demon King",
        };


        dialogueManager.StartDialogue(dialogueLineswithBoss, speakers);

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

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(false);
        }

        EnablePlayerMovement();

        yield return new WaitForSeconds(1.5f);

        if (finalBoss != null)
        {
            finalBoss.enabled = true; //enables the script
            finalBoss.fightStarted = false;
            finalBoss.StartBossFight();
        }
    }
    private void EnablePlayerMovement()
    {
        if(pa != null)
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

    private IEnumerator EnablePlayer()
    {
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(false);
        }

        EnablePlayerMovement();
        cutsceneStarted = false;
    }

    public void StartAfterTeleportBossCutScene()
    {
        StartCoroutine(AfterTeleportBossCutScene());
    }

    private IEnumerator AfterTeleportBossCutScene()
    {
        yield return new WaitForSeconds(1f);

        DisablePlayerMovement();

        dialogueManager.gameObject.SetActive(true);

        string[] teleportDialogue = {
            "(Arrogant, cruel delight in his tone)\n Behold... the family you beld for. The king who cast you aside. The jewel who wrote to ghost. A kingdom's last treasures, trussed up like sacrifices.",//dk
            "(Struggling against the chains, voice breaking)\n Stop this! Let him go! Please....",//p
            "(Glances at her, smirking)\n Such spirit. You beg for your father's life while your knight bleeds for yours. How delicious.",//dk
            "(Lifting his head weakly, voice craked with age and bitterness)\n You... Knight... Why do your stand here? You should have stayed buried. You were nothing but a stain on our history.",//King
            "(Voice hollow but firm)\n And Yet... here I am. Not for you. Never for you.",//k
            "(Desperate, eyes locked on the Knight)\n Don't listen to him. Please... you're all that's left. Do you know why I sent that letter? Why I broke every oath to summon you back?",//p
            "(Shakes her head, tears running down her face)\n No! I sent it because I remembered. When the world spat on you, I remembered the man who stayed behind in the courtyard while others feasted.\n " +
            "The one who listened when no one else saw me as more than a crown. I couldn't let you vainsh, not when I... not when I needed you most.",//p
            "(Looks at her, voice low, tinged with sorrow)\n And I answered... becuase I never stopped hearing you.",//K
            "(Choking out a laugh, striding closer to the throne)\n Yes. All that's left of a broken tale. And every story needs its ending.",//dk
            "(He raises his Axe, flames forming to strike the King.)",//dk
            "(Closing his eyes, whispering bitterly)\n So this is my punishment... dying in the shadow of a traitor.",//King
            "(Screaming, thrashing against her bonds)\n No!",//p
        };

        string[] speakers = {
             "Demon King",
             "Princess Elara",
             "Demon King",
             "King",
             "Knight",
             "Princess Elara",
             "Princess Elara",
             "Knight",
             "Demon King",
             " ",
             "King",
             "Princess Elara",
        };

        dialogueManager.StartDialogue(teleportDialogue, speakers);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());
        dialogueManager.gameObject.SetActive(false);


        
        yield return DemonKingMoveToPosition(kingPoint.position);
        finalBoss.GetComponent<Animator>().SetBool("Attack", true);

        //rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        yield return KnightMoveToPosition(knightPoint.position);

        yield return new WaitForSeconds(0.6f);

        finalBoss.GetComponent<Animator>().SetBool("Attack", false);

        playerController.GetComponent<playercontroller>().TriggerPrayAnimation();

        yield return new WaitForSeconds(1f);
        dialogueManager.gameObject.SetActive(true);

        string[] secondHalfDialogue = {
            "(Shrieking, tears streaming)\n NO! Please.. don't! You can't",//p
            "(Voice ragged, straining, forcing the words through blood and pain)\n I told you... I'd bury the world... before I let them take you.",//K
            "(Snarling, more amused than enraged)\n Hahahaha! Look at him crawl. Even gutted, the ghost clings to his illusion. Does it hurt, knight? To bleed for the same crown that left you to rot?",//dk
            "(Weak but unwavering, lifting his sword with his sword with trembling hands)\n I bleed for her. Not for you. Not for him. Just her.",//K
            "(Choking through sobs, desperate, her voice breaking apart)\n Why? Why would you do this? AFter everything they did to you... after all they took... why still throw yourself into the fire for me?",//p
            "(Meets her eyes, bloodied smile faint but resolute)\n Because... even in the silence... even when the world turned its back... you never did. \n" +
            "You remembered me. You believed when no one else would. And that... is worth more than a crown, more than a kingdom, more than my life.",//K
            "(Whispering through tears, heart shattering)\n You can back for me... You never stopped...",//p
            "(Snarling, voice echoing with venom)\n Then watch, little cnadle. Watch as your knight's last flame burns out at my feet.",//dk
    };

        string[] secondSpeakers = {
            "Princess Elara",
            "Knight",
            "Demon King",
            "Knight",
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Demon King",
    };

        dialogueManager.StartDialogue(secondHalfDialogue, secondSpeakers);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());


        EnablePlayerMovement();

        dialogueManager.gameObject.SetActive(false);
        finalBoss.enabled = true;
        finalBoss.StartBossFight();
    }

    public void TheEndingSequence()
    {
        StartCoroutine(TheEndingdSequence());
    }

    private IEnumerator TheEndingdSequence()
    {
        yield return new WaitForSeconds(2f);

        DisablePlayerMovement();

        dialogueManager.gameObject.SetActive(true);

        string[] postFightDialogue = {
            "(Weak laugh, voice hollow and bitter)\n So... this is how it ends... The forgotten knight... topples a king of darkness...",//dk
            "(Breathless, holding his sword like it weighs the world)\n It was never your throne. Only a prison... for all of us.",//k
            "(Snarls, then smirks through blood)\n Prison... or mirror? You and I... not so different... abandoned... betrayed... drowning in silence.",//dk
            "(Quiet, firm, eyes glowing faintly)\n You chained yourself to hate. I... chose to overcome.",//k
            "(Exhales one last broken laugh, head lowering)\n Then... perhaps... you've already won...",//dk
        };

        string[] speakers = {
            "Demon King",
            "Knight",
            "Demon King",
            "Knight",
            "Demon King",
        };

        dialogueManager.StartDialogue(postFightDialogue, speakers);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        StartCoroutine(PrincessMoveToPosition(princess, playerSprite.transform, pmoveSpeed, groundLayer));

        yield return new WaitForSeconds(2f);

        string[] endingDialogue = {
            "(sobbing, holding him close)\n No... no, please, don't leave me. Not now, not after everything... You can't",//p
            "(Voice faint but steady, forcing a small smile)\n I was strong enough... to protect you. That's all I ever wanted... all that mattered.",//k
            "(Desperate, tears falling onto his armor)\n You were more than that. You were hope. You were... my heart. Why did the world have to forget you? Why did they throw you away?",//p
            "(Eyes distant, pained)\n I carried it all... the silence, the betrayal... the emptiness. Every day I walked alone... rust eating at me... until your letter found me. You remembered me... when on one else did. That was enough to keep me standing.",//k
            "(Shaking her head, holding his hand tightly)\n You deserved more than being a memory. You deserved a life... a future.",//p
            "(Smiling faintly through blood, his vioce breaking)\n I dreamed of it... a house with you far from battle... laughter in the halls instead of silence... waking each day with you by my side. A world where I wasn't a ghost... but a man",//k
            "(Sobbing harder, pressing her forehead to his)\n We could have had that. We should have had that.",//p
            "(Whisper, his strength fading)\n I'm sorry... for leaving you like this. I wanted... to be with you forever... but this is all I have left.",//k
            "(Clutching him as if to keep his soul from slipping away)\n Don't say it... don't you dare!",//p
            "(Last breath, whispering with all his heart)\n Thank you... for remembering me... for loving me. I... loovveee... youu.. always....",//k
        };

        string[] speakerstwo = {
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Knight",
        };

        dialogueManager.StartDialogue(endingDialogue, speakerstwo);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        yield return mainFade.FadeOut();

        mainCam.gameObject.SetActive(false);

        endingCam.gameObject.SetActive(true);

        yield return endingFade.FadeIn();

        string[] epilogueDialogue = {
            "(soft, voice trembling but steady)\n The world is at peace... becuase of you. The people sing your name, though most will never know the weight you carried... or the loneliness you endured. " +
            "\n But to me... you were my love. The man who loved when the world had forgotten how. I'll never forget",//p
            "You should be here, walking in the light you gave back to us.",//p
            "But instead, I'll carry your dream. I build the home you wished for.",//p
            "And in ever stone, in every flower... you'll be there with me.",//p
            "You weren't just my knight...",//p
            "You were my heart.",//p
            "And even though you're gone... I'll love you for the rest of my days.",//p
            "(She lays her hand gently on the tombstone, closes her eyes, and whispers one last thing.)\n Rest now... My love.",//p
        };

        string[] speakersthree = {
            "Princess Elara",
            "Princess Elara",
            "Princess Elara",
            "Princess Elara",
            "Princess Elara",
            "Princess Elara",
            "Princess Elara",
            "Princess Elara",
        };

        dialogueManager.StartDialogue(epilogueDialogue, speakersthree);

        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        dialogueManager.gameObject.SetActive(false);

        Debug.Log("loading to the next level");

        SceneManager.LoadScene(nextLevel);
    }

    private IEnumerator KnightMoveToPosition(Vector3 targetPosition)
    {
        // Adjust target to maintain Z position
        Vector3 target = new Vector3(targetPosition.x, playerSprite.position.y, playerSprite.position.z);

        //FaceTarget(playerSprite, knightPoint);

        while (Mathf.Abs(playerSprite.position.x - target.x) > 0.1f)
        {
            // Move towards the target
            playerSprite.position = Vector3.MoveTowards(playerSprite.position, target, kmoveSpeed * Time.deltaTime);
            yield return null;
        }

        playerSprite.position = target; // Snap to the target position
    }

    private IEnumerator DemonKingMoveToPosition(Vector3 targetPosition)
    {
        // Adjust target to maintain Z position
        Vector3 target = new Vector3(targetPosition.x, bossSprite.position.y, bossSprite.position.z);

        //FaceTarget(bossSprite, kingPoint);

        while (Mathf.Abs(bossSprite.position.x - target.x) > 0.1f)
        {
            // Move towards the target
            bossSprite.position = Vector3.MoveTowards(bossSprite.position, target, dkmoveSpeed * Time.deltaTime);
            yield return null;
        }

        bossSprite.position = target; // Snap to the target position
    }

    private IEnumerator PrincessMoveToPosition(Transform princess, Transform player, float speed, LayerMask groundLayer)
    {
        if (princess == null || playerSprite == null)
        {
            Debug.LogWarning("Princess or player not assigned.");
            yield break;
        }
        float stopDistance = 0.5f;

        while (Vector2.Distance(new Vector2(princess.position.x, 0), new Vector2(player.position.x, 0)) > stopDistance)
        {
            Vector3 targetPos = new Vector3(player.position.x, princess.position.y, princess.position.z);
            princess.position = Vector3.MoveTowards(princess.position, targetPos, speed * Time.deltaTime);

            RaycastHit2D hit = Physics2D.Raycast(princess.position + Vector3.up * 0.5f, Vector2.down, 3f, groundLayer);

            if (hit.collider != null)
            {
                SpriteRenderer sr = princess.GetComponent<SpriteRenderer>();
                float footOffset = 0f;

                if (sr != null)
                {
                    footOffset = sr.bounds.extents.y;
                }

                princess.position = new Vector3(princess.position.x, hit.point.y + footOffset, princess.position.z);
            }

            yield return null;
        }

        Vector3 finalPos = princess.position;
        finalPos.x = player.position.x;

        // Recheck ground one last time to ensure perfect grounding
        RaycastHit2D finalHit = Physics2D.Raycast(finalPos + Vector3.up * 0.5f, Vector2.down, 3f, groundLayer);
        if (finalHit.collider != null)
        {
            SpriteRenderer sr = princess.GetComponent<SpriteRenderer>();
            float footOffset = sr != null ? sr.bounds.extents.y : 0f;
            finalPos.y = finalHit.point.y + footOffset;
        }

        princess.position = finalPos;
        FaceTarget(princess); 
    }

    private void FaceTarget(Transform target)
    {
        if (spriteRender != null)
        {
            spriteRender.flipX = target.position.x < transform.position.x;
        }
    }

}
