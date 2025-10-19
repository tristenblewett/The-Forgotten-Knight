using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchSound : MonoBehaviour
{
    public AudioSource fireSound;
    public Transform player;
    public float maxDistance = 10f;
    private float initialVolume;


    private void Start()
    {
        initialVolume = fireSound.volume;
    }

    private void Update()
    {
        if(fireSound != null && fireSound.isPlaying == false)
        {
            fireSound.Play();
        }

        float distance = Vector3.Distance(player.position, transform.position);

        if(distance < maxDistance)
        {
            fireSound.volume = Mathf.Lerp(0, initialVolume, (maxDistance - distance) / maxDistance);
        }
        else
        {
            fireSound.volume = 0f;
        }
    }
}
