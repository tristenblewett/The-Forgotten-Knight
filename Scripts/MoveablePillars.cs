using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveablePillars : MonoBehaviour
{
    public List<GameObject> pillars; // List to hold multiple pillar objects
    public float height = 10f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float speed = 3f;

    private List<Vector3> startPositions = new List<Vector3>();
    private float crushZoneHeight = 0.01f; // Height of the crushing area beneath the pillar
    private float crushZoneWidthReduction = 2f; // Amount to reduce width from each side of the pillar
    public float topDetectionOffset = -0.4f;

    [SerializeField] private playercontroller player;

    private void Start()
    {
        player = GetComponent<playercontroller>();
        // Store the start position of each pillar
        foreach (GameObject pillar in pillars)
        {
            startPositions.Add(pillar.transform.position);
            StartCoroutine(ControlPillar(pillar, startPositions[startPositions.Count - 1]));
        }
    }

    private IEnumerator ControlPillar(GameObject pillar, Vector3 startPos)
    {
        bool isRising = true;

        while (true)
        {
            Vector3 targetPos = isRising ? startPos + Vector3.up * height : startPos;

            // Move pillar to the target position
            while (Vector3.Distance(pillar.transform.position, targetPos) > 0.1f)
            {
                pillar.transform.position = Vector3.MoveTowards(pillar.transform.position, targetPos, speed * Time.deltaTime);
                yield return null;

                CheckForPlayer(pillar);
            }

            // Wait for a random time before switching direction
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

            isRising = !isRising;
        }
    }

    private void CheckForPlayer(GameObject pillar)
    {
        // Get the CompositeCollider2D attached to the pillar
        CompositeCollider2D compositeCollider = pillar.GetComponent<CompositeCollider2D>();
        if (compositeCollider == null)
        {
            Debug.LogError($"Pillar {pillar.name} does not have a CompositeCollider2D!");
            return;
        }

        // Calculate the crushing zone below the pillar
        Bounds bounds = compositeCollider.bounds;

        // Shrink the width and calculate crush zone
        float crushZoneLeft = bounds.min.x + crushZoneWidthReduction;
        float crushZoneRight = bounds.max.x - crushZoneWidthReduction;
        float crushZoneTop = bounds.min.y;
        float crushZoneBottom = bounds.min.y - crushZoneHeight;

        // Use OverlapAreaAll to detect players within the crushing zone
        Collider2D[] hits = Physics2D.OverlapAreaAll(
            new Vector2(crushZoneLeft, crushZoneTop),
            new Vector2(crushZoneRight, crushZoneBottom),
            LayerMask.GetMask("Player")
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player crushed!");
                PlayerCrushed(hit.gameObject);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //player.audioSource.mute = true;
            player.onMovingPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //player.audioSource.mute = false;
            player.onMovingPlatform = false;
        }
    }

    private void PlayerCrushed(GameObject player)
    {
        // Get the player's health script and handle death
        playerHealth playerhp = player.GetComponent<playerHealth>();
        if (playerhp != null)
        {
            playerhp.Die();
        }
        else
        {
            Debug.LogError("Player object does not have a playerHealth component!");
        }
    }
    
    private void OnDrawGizmos()
    {
        // Visualize the crushing area for debugging purposes
        Gizmos.color = Color.red;
        foreach (GameObject pillar in pillars)
        {
            if (pillar != null)
            {
                CompositeCollider2D compositeCollider = pillar.GetComponent<CompositeCollider2D>();
                if (compositeCollider != null)
                {
                    Bounds bounds = compositeCollider.bounds;

                    // Shrink the width and calculate crush zone for visualization
                    float crushZoneLeft = bounds.min.x + crushZoneWidthReduction;
                    float crushZoneRight = bounds.max.x - crushZoneWidthReduction;
                    float crushZoneTop = bounds.min.y;
                    float crushZoneBottom = bounds.min.y - crushZoneHeight;

                    Vector2 center = new Vector2((crushZoneLeft + crushZoneRight) / 2, (crushZoneTop + crushZoneBottom) / 2);
                    Vector2 size = new Vector2(crushZoneRight - crushZoneLeft, crushZoneHeight);

                    Gizmos.DrawWireCube(center, size);

                    
                    Gizmos.color = Color.yellow; 
                    Vector2 topCenter = new Vector2(bounds.center.x, bounds.max.y + topDetectionOffset);
                    Vector2 topSize = new Vector2(bounds.size.x, 0.2f);

                    Gizmos.DrawWireCube(topCenter, topSize);
                    Gizmos.color = Color.red; // Reset color for next pillar
                }
            }
        }
    }
}
