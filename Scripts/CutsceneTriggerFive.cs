using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneTriggerFive : MonoBehaviour
{
    [SerializeField] private CutsceneMangerAfterLevelThree cutsceneMangerAfterLevelThree;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cutsceneMangerAfterLevelThree.StartCutScene();

            gameObject.SetActive(false);
        }
    }
}
