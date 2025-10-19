using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerFour : MonoBehaviour
{
    [SerializeField] private CutsceneMangerafterBossFightTwo cutsceneMangerafterBossFightTwo;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cutsceneMangerafterBossFightTwo.StartCutScene();

            gameObject.SetActive(false);
        }
    }
}
