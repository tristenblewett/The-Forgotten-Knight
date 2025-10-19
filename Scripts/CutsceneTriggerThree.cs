using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerThree : MonoBehaviour
{
    public CutsceneMangerAfterLevelTwo cutsceneManagerthree;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cutsceneManagerthree.StartCutScene();

            gameObject.SetActive(false);
        }
    }
}
