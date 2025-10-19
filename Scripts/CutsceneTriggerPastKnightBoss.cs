using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerPastKnightBoss : MonoBehaviour
{
    public CutSceneMangerPastKnightBoss cutsceneManagerpastknightboss;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cutsceneManagerpastknightboss.StartCutscene();

            gameObject.SetActive(false);
        }
    }
}
