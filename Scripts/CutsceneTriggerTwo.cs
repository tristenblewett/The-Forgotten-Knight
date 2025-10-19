using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerTwo : MonoBehaviour
{
    public CutsceneMangerafterBossFightOne cutsceneManagertwo;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cutsceneManagertwo.StartCutScene();

            gameObject.SetActive(false);
        }
    }
}
