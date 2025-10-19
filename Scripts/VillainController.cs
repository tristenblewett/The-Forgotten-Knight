using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillainController : MonoBehaviour
{
    public DialogueManager dialogueManager; // Reference to handle dialogue
    [SerializeField] private CutsceneManger cutsceneManger;
    public Transform princessTransform; // Princess's position
    public GameObject princessObject;
    public Transform portalExitPoint; // Portal's position
    public GameObject portal; // Visual portal effect
    public float moveSpeed = 3f; // Movement speed

    private bool isVillainActive = false;

    [SerializeField] private AudioClip portalActivationSound;
    [SerializeField] private AudioClip portalIdleSound;
    [SerializeField] private AudioSource portalAudioSource;

    private void Start()
    {
        // Ensure the villain and portal are inactive at the start
        gameObject.SetActive(false);
        if (portal != null)
        {
            portal.SetActive(false);
        }
    }

    public void ActivateVillain()
    {
        // Portal activation
        if (portal != null)
        {
            portal.transform.position = portalExitPoint.position;
            portal.SetActive(true);

            if(portalAudioSource != null && portalActivationSound != null)
            {
                portalAudioSource.clip = portalActivationSound;
                portalAudioSource.Play();
            }
        }

        gameObject.SetActive(true);
        StartCoroutine(VillainScene());
    }

    private IEnumerator PlayPortalIdle()
    {
        yield return new WaitForSeconds(portalActivationSound.length);

        if(portalAudioSource != null && portalIdleSound != null)
        {
            portalAudioSource.clip = portalIdleSound;
            portalAudioSource.loop = true;
            portalAudioSource.Play();
        }
    }

    private IEnumerator VillainScene()
    {

        if (portalAudioSource != null && portalIdleSound != null)
        {
            StartCoroutine(PlayPortalIdle());
        }

        // Villain's entrance and initial dialogue
        yield return MoveToPrincess();

        // Dialogue: Demon Prince addressing the knight and princess
        string[] villainLines = {
            "(Sneering, his voice echoing like thunder)\nWhat a touching reunion. The broken knight and the desperate princess, clinging to memories of a world long gone.",
            "(Laughs, a dark, guttural sound)\nOh, but I am. Did you think you could plot against my master without me noticing?"
        };
        string[] villainSpeakers = { "Demon Prince", "Demon Prince" };
        dialogueManager.StartDialogue(villainLines, villainSpeakers);
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        // Dialogue: Princess's reaction
        string[] princessLines = {
            "(Turning toward the Demon Prince, her voice faltering, filled with fear)\nYou... You’re not supposed to be here.",
            "(Her voice shaking, tears welling in her eyes)\nPlease... leave us."
        };
        string[] princessSpeakers = { "Princess Elara", "Princess Elara" };
        dialogueManager.StartDialogue(princessLines, princessSpeakers);
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        // Dialogue: Demon Prince threatening the princess
        string[] threatLines = {
            "(Snarling, his voice dripping with malice)\nOh, I’m not here to kill you, princess. Not yet. My master has plans for you."
        };
        string[] threatSpeakers = { "Demon Prince" };
        dialogueManager.StartDialogue(threatLines, threatSpeakers);
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        // Kidnapping sequence
        StartCoroutine(KidnapPrincess());
    }

    private IEnumerator KidnapPrincess()
    {
        // Move to the princess
        yield return MoveToPrincess();

        // Grab the princess
        princessTransform.SetParent(transform);
        princessTransform.localPosition = new Vector3(1, 1, 0); // Adjust position relative to the Demon Prince

        // Villain mocks the knight
        string[] tauntLines = {
            "(Mockingly, to the knight)\nStill the coward, I see. How pathetic."
        };
        string[] tauntSpeakers = { "Demon Prince" };
        dialogueManager.StartDialogue(tauntLines, tauntSpeakers);
        yield return new WaitUntil(() => dialogueManager.IsDialogueFinished());

        
        // Wait for portal effect
        yield return new WaitForSeconds(2f);

        // Move to portal exit with the princess
        yield return MoveToPosition(portalExitPoint.position);

        if (portalAudioSource != null && portalAudioSource.isPlaying)
        {
            portalAudioSource.Stop(); // Stop the idle sound
        }

        // Portal activation
        if (portal != null)
        {
            portal.transform.position = portalExitPoint.position;
            portal.SetActive(false);
        }

        // Remove the villain and princess
        Destroy(gameObject);

        cutsceneManger.EnablePlayer();
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        // Adjust target to maintain Z position
        Vector3 target = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        while (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(target.x, target.y)) > 0.1f)
        {
            // Move towards the target
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target; // Snap to the target position
    }

    private IEnumerator MoveToPrincess()
    {
        // Retrieve the position of the princess's GameObject
        Vector3 targetPosition = princessObject.transform.position;
        Vector3 target = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        while (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(target.x, target.y)) > 0.1f)
        {
            // Update the target position in case the princess moves
            targetPosition = princessObject.transform.position;
            target = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

            // Move towards the princess
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = target; // Snap to the princess's position
    }

    public bool IsVillainSequenceFinished()
    {
        return isVillainActive; // This should be a flag set at the end of the villain's sequence
    }
}
