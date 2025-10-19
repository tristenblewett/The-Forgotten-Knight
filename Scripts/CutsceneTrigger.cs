using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    public CutsceneManger cutsceneManager;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            cutsceneManager.StartCutscene();

            gameObject.SetActive(false);
        }
    }
}
