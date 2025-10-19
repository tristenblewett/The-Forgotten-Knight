using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToBossTwoFightTransition : MonoBehaviour
{
    private string BossTwoScene = "2ndBoss";
    private bool hasFinishedTalking = false;

    public void FinishDialogue()
    {
        hasFinishedTalking = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && hasFinishedTalking)
        {
            SceneManager.LoadScene(BossTwoScene);
        }
    }
}
