using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveablePlatforms : MonoBehaviour
{
    public float speed = 3f;
    public float range = 5f;
    public float waitTime = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingRight = true;

    [SerializeField] private playercontroller player;

    private void Start()
    {
        player = GetComponent<playercontroller>();

        startPos = transform.position;
        targetPos = startPos + Vector3.right * range;
        StartCoroutine(MovePlatform());
    }

    private IEnumerator MovePlatform()
    {
        while(true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if(Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                movingRight = !movingRight;
                targetPos = movingRight ? startPos + Vector3.right * range : startPos - Vector3.right * range;

                yield return new WaitForSeconds(waitTime);
            }

            yield return null;
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
}
