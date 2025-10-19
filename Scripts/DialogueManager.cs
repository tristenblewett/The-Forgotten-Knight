using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI letterText; // Text field for the dialogue
    [SerializeField] private TextMeshProUGUI speakerNameText; // Text field for the speaker's name
    [SerializeField] private float typingSpeed = 0.05f; // Typing speed
    private string[] currentDialogueLines; // Dialogue lines for the current conversation
    private string[] currentSpeakers; // Corresponding speakers for the current conversation
    private int currentLineIndex = 0; // Tracks the current line being displayed
    private bool isTyping = false; // Prevents skipping dialogue while typing
    private bool isDialogueActive = false; // Ensures dialogue only plays once
    private bool isDialogueFinished = false; // Tracks when dialogue ends
    private Coroutine typingCoroutine;
    // Start the dialogue sequence
    public void StartDialogue(string[] dialogueLines, string[] speakers)
    {
        if (isDialogueActive) return; // Prevent re-triggering dialogue

        isDialogueActive = true; // Mark dialogue as active
        isDialogueFinished = false; // Reset dialogue finished flag
        currentDialogueLines = dialogueLines;
        currentSpeakers = speakers;
        currentLineIndex = 0; // Start from the first line
        letterText.text = ""; // Clear any existing text
        speakerNameText.text = ""; // Clear any existing speaker name
        DisplayNextLine();
    }

    // Handles displaying the next line of dialogue
    private void DisplayNextLine()
    {
        if (currentLineIndex >= currentDialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        // Set the speaker's name
        speakerNameText.text = currentSpeakers[currentLineIndex];

        // Start typing the current line
        isTyping = true;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLines(currentDialogueLines[currentLineIndex]));
    }


    // Typewriter effect for displaying letters one by one
    private IEnumerator TypeLines(string line)
    {
        letterText.text = ""; // Clear the text box before typing
        foreach (char letter in line)
        {
            letterText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Ends the dialogue sequence
    private void EndDialogue()
    {
        letterText.text = ""; // Clear dialogue text
        speakerNameText.text = ""; // Clear speaker name
        isDialogueActive = false; // Reset dialogue active flag
        isDialogueFinished = true; // Mark dialogue as finished
        Debug.Log("Dialogue finished!");
    }

    // Allows skipping the line while typing
    private void Update()
    {
        if (!isDialogueActive) return; // Ignore input if dialogue is inactive

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // Finish typing the current line instantly
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                letterText.text = currentDialogueLines[currentLineIndex];
                isTyping = false;
            }
            else
            {
                // Move to the next line
                currentLineIndex++;
                DisplayNextLine();
            }
        }
    }
    // Checks if dialogue has finished
    public bool IsDialogueFinished()
    {
        return isDialogueFinished;
    }
}
