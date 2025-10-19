using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Teleporter : Interactable
{
    [SerializeField] private Transform[] teleportPoints;
    [SerializeField] private CinemachineVirtualCamera cam;
    protected override void Interact()
    {
        if(teleportPoints.Length > 0)
        {
            Transform randomPoint = teleportPoints[Random.Range(0, teleportPoints.Length)];

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null)
            {
                player.transform.position = randomPoint.position;

                UpdateCameraFollow(player.transform);

                Debug.Log("player teleported to:" + randomPoint.name);
            }
            else
            {
                Debug.LogWarning("No player found with tag 'Player' ");
            }
        }
        else
        {
            Debug.LogWarning("No Teleport points assigned to the teleporterInteractable");
        }
    }

    private void UpdateCameraFollow(Transform playerTransform)
    {
        CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
        if (cam != null)
        {
            cam.Follow = playerTransform;
            Debug.Log("Camera follow updated to: " + playerTransform.name);
        }
        else
        {
            Debug.LogWarning("No CinemachineVirtualCamera found in the scene!");
        }
    }
}
