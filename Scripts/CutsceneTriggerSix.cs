using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerSix : MonoBehaviour
{
    [SerializeField] private CutsceneMangerafterBossFightThree CutsceneMangerafterBossFightThree;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CutsceneMangerafterBossFightThree.StartCutScene();

            gameObject.SetActive(false);
        }
    }
}
