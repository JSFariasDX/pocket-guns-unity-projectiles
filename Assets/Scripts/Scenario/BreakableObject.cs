using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BreakableObject : MonoBehaviour
{
    [Header("Dependencies")]
    public GameObject puffEffect;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private List<AudioClip> breakSfx = new List<AudioClip>();

    [Header("Settings")]
    public bool isBreakable = true;
    [SerializeField] private float maxHealth = 50;
    [SerializeField] private float defaultExplosionForce = 50f;

    private float _currentHealth;

    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    Explodable explode;

    bool isDestroyed = false;

    void Start()
    {
        _currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        explode = GetComponent<Explodable>();

        explode.fragmentInEditor();
    }

	public void OnDamage(float damage, Player p = null)
    {
        if (!isBreakable)
            return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            isDestroyed = true;
			explode.generateFragments();
			explode.explode();

			DropObject dropObject = GetComponent<DropObject>();
            if (dropObject)
			{
                dropObject.Drop(p);
			}
            if (breakSfx.Count > 0) audioSource.PlayOneShot(breakSfx[Random.Range(0, breakSfx.Count)]);

			ExplodePieces();
		} 
        else if (!isDestroyed)
        {
            StartCoroutine("DamageFeedback");
        }
    }

    public void ExplodePieces()
    {
        GameObject puff = Instantiate(puffEffect, transform.position, Quaternion.identity);
        Destroy(puff, .5f);

        Collider2D[] inExplosionRadius = Physics2D.OverlapCircleAll(transform.position, 5);

        foreach (Collider2D item in inExplosionRadius)
        {
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();

            if (rb == null)
                continue;

            var direction = item.transform.position - transform.position;
            var distance = Vector2.Distance(item.transform.position, transform.position);

            if(distance <= 0)
                continue;
            
            float explosionForce = defaultExplosionForce / distance;
            rb.AddForce(direction.normalized * explosionForce);

            if(item.GetComponent<MeshRenderer>())
                item.GetComponent<MeshRenderer>().material.SetInt("_Hit", 0);
        }
    }

    IEnumerator DamageFeedback()
    {
        spriteRenderer.material.SetInt("_Hit", 1);
        transform.position += new Vector3(-.1f, 0, 0);
        yield return new WaitForSeconds(.05f);
        transform.position += new Vector3(.1f, 0, 0);
        audioSource.PlayOneShot(damageSound);
        yield return new WaitForSeconds(0.1f);
        transform.position += new Vector3(.1f, 0, 0);
        yield return new WaitForSeconds(.05f);
        transform.position += new Vector3(-.1f, 0, 0);
        spriteRenderer.material.SetInt("_Hit", 0);
    }
}
