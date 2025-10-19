using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LogTrigger : MonoBehaviour
{

    [SerializeField] private FinalBoss finalBoss;
    [SerializeField] private GameObject log;
    [SerializeField] private float activationDelay = 2f;
    private bool hasActivated = false;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player"))
        {
            hasActivated = true;
            StartCoroutine(ActivateTriggerAfterDelay());
        }
    }

    private IEnumerator ActivateTriggerAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        if (log != null)
        {
            log.SetActive(true);
        }

        if (finalBoss != null)
        {
            finalBoss.StartTunnelEnemyWave();
        }
    }

}
