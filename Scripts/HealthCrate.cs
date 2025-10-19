using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealthCrate : Interactable
{
    private int healthToRestore = 25;
   [SerializeField] private GameObject healthPotion;
    [SerializeField] private Light2D CrateLight;
    private float popUpHeight = 2f;
    private float popUpDuration = 0.5f;
    private float destroyDelay = 1f;

    [SerializeField] private playerHealth playerHP;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] openCrateSound;

    private void Start()
    {
        CrateLight = GetComponent<Light2D>();
        playerHP = FindAnyObjectByType<playerHealth>();

        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (healthPotion != null)
        {
            healthPotion.SetActive(false);
        }
    }

    protected override void Interact()
    {
        if(playerHP != null)
        {
            if(playerHP.IsAtFullHealth())
            {
                return;
            }


            playerHP.Heal(healthToRestore);

            if(audioSource != null && openCrateSound.Length > 0)
            {
                int randomIndex = Random.Range(0, openCrateSound.Length);
                audioSource.PlayOneShot(openCrateSound[randomIndex]);
            }

            if(healthPotion != null)
            {
                healthPotion.SetActive(true);
                if(CrateLight != null)
                {
                    CrateLight.enabled = false;
                }

                StartCoroutine(PopUpPotion());
            }
        }
    }

    private IEnumerator PopUpPotion()
    {
        Vector3 OriginalPostion = healthPotion.transform.position;
        Vector3 targetPosition = OriginalPostion + new Vector3(0f, popUpHeight, 0f);

        float elaspedTime = 0f;

        while(elaspedTime < popUpDuration)
        {
            healthPotion.transform.position = Vector3.Lerp(OriginalPostion, targetPosition, (elaspedTime / popUpDuration));
            elaspedTime += Time.deltaTime;

            yield return null;
        }

        healthPotion.transform.position = targetPosition;

        yield return new WaitForSeconds(destroyDelay);

        Destroy(healthPotion);
        Destroy(gameObject);
    }
}
