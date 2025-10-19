using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerFinalTwo : MonoBehaviour
{
    [SerializeField] private CutSceneMangerFinal cutSceneMangerFinal;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cutSceneMangerFinal.StartCutsceneWithBoss();
            Debug.Log("player has enter the cutscene");

            gameObject.SetActive(false);
        }
    }
}
