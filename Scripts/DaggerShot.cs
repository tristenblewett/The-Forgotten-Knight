using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerShot : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rigid;
    public float force;

    private float timer;

    [SerializeField] private LayerMask blockingLayer;
    private int DamagetoPlayer = 5;

    [SerializeField] private AudioClip blockSound;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();

        Shooting();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > 10)
        {
            Destroy(gameObject);
        }

        CheckForBlockingPillar();
    }

    private void Shooting()
    {
        Vector3 direction = player.transform.position - transform.position;
        rigid.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    private void CheckForBlockingPillar()
    {
        Vector3 direction = rigid.linearVelocity.normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, blockingLayer);
        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.blue, 2f);
            //playing the block sound
            if(audioSource != null && blockSound != null)
            {
                audioSource.PlayOneShot(blockSound);
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<playerHealth>().PlayerTakeDamage(DamagetoPlayer);
            Destroy(gameObject);
        }
    }
}
