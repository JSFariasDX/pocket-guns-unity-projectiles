using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    int currentHits = 0;
    [SerializeField]
    int maxHits = 3;

    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    public AudioClip damageSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            StartCoroutine("DamageFeedback");
            currentHits++;
        }
        if (currentHits >= maxHits)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DamageFeedback()
    {
        spriteRenderer.material.SetInt("_Hit", 1);
        audioSource.PlayOneShot(damageSound);
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.material.SetInt("_Hit", 0);
    }
}
