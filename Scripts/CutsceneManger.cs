using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManger : MonoBehaviour
{
    public DialogueManager dialogueManager; // DialogueManager reference
    public VillainController villainController; // VillainController reference
    public Transform portalSpawnPoint; // Location for the portal to spawn
    public GameObject portalPrefab; // Prefab for the portal
    public CutsceneDoor doorController; // DoorController reference
    public GameObject player;

    private playercontroller playerController;
    private Rigidbody2D rigid;
    [SerializeField] private Animator playerAnimator;
    private playeranimator pa;

    private bool cutsceneStarted = false;

    private void Start()
    {
        if (portalPrefab != null)
        {
            portalPrefab.SetActive(false);
        }

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(false);
        }

        if(player != null)
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

        if (playerController != null)
        {
            DisablePlayerMovement();
            /*
            if(playerAnimator != null)
            {
                playerAnimator.Play("idle");
            }
            */
        }

        if (dialogueManager != null)
        {
            dialogueManager.gameObject.SetActive(true);
        }

        // Princess and Knight dialogue
        string[] dialogueLines = {
            "(Softly, her voice trembling yet resolute)\nYou came. I didn�t think you�d answer my letter.",
            "(His tone is low, almost bitter, his eyes hidden beneath his helmet)\nI almost didn�t. Do you know how long it�s been since anyone said my name? Since anyone remembered me?",
            "(Taking a step closer, her gaze steady but sorrowful)\nI never forgot you. Not once. Even when the world turned its back, I held onto hope.",
            "(Letting out a hollow laugh, his gauntlet tightening around his sword hilt)\nHope? Hope died the day they cast me aside. The \"King's Champion,\" reduced to nothing more than a ghost in rusted armor. And now, you summon me as though I�m your last resort.",
            "(Desperation creeping into her voice)\nBecause you are. The kingdom is crumbling, and the Chaos Demon Army marches to destroy everything we�ve ever known. Who else can stand against them?",
            "(Pauses, his voice softening, tinged with pain)\nWhy me, then? You could have called for any knight. Any hero.",
            "(Firmly, stepping even closer)\nBecause you were the best of us. You fought for the kingdom, not for glory or gold. You fought for what was right.",
            "(Shaking his head)\nThat knight died a long time ago, Princess. I�m not the man you remember."
        };

        string[] speakers = {
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Knight",
            "Princess Elara",
            "Knight"
        };

        // Start the Princess and Knight dialogue
        dialogueManager.StartDialogue(dialogueLines, speakers);

        // Wait for the dialogue to end, then trigger the villain's entrance
        StartCoroutine(WaitForDialogueThenVillain());
    }

    private IEnumerator WaitForDialogueThenVillain()
    {
        // Wait until the Princess and Knight dialogue finishes
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        // Trigger the Villain's entrance
        villainController.ActivateVillain();

        // Wait for the villain sequence (handled in VillainController)
        yield return new WaitUntil(() => villainController.IsVillainSequenceFinished());

        doorController.EnableInteraction();
        Debug.Log("door interaction enabled.");

        //reenables the player movement
        EnablePlayerMovement();
        

        
    }

    private void SummonPortal()
    {
        Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);
        Debug.Log("Portal summoned!");
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
    public void EnablePlayer()
    {
        EnablePlayerMovement();
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
