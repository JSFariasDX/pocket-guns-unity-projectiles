using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WallLight : MonoBehaviour
{
    public bool active = true;
    public SpriteRenderer spriteRenderer;
    public Sprite[] brokenSprites;
    private int damage;
    [SerializeField]
    ParticleSystem particle;
    [SerializeField]
    bool isBreakable = true;
    [SerializeField] AudioClip breakSfx;

    private void Start()
    {
        if (!active) return;
        var main = particle.main;
        int sorted = Random.Range(5, 20);
        main.maxParticles = sorted;
    }

    public void SetPlacementDirection(int value)
    {
        if (value == 0)
        {
            SetRotation(180f);
        } 
        else if (value == 1)
        {
            SetRotation(90f, .3f, .5f);
        } 
        else if (value == 2)
        {
            SetRotation(0, .5f, .7f);
        } 
        else if (value == 3)
        {
            SetRotation(-90f, .7f, .5f);
        }
    }

    public void SetRotation(float angle, float offsetX = .5f, float offsetY = .3f)
    {
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.position = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, 0);
    }

    public void OnDamage()
    {
        if (!isBreakable)
        {
            return;
        } 
        else
        {
            HandleDamage();
        }
    }

    void HandleDamage()
    {
        if (damage > 1)
        {
            Destroy(GetComponent<Collider2D>());
            Destroy(GetComponent<Light2D>());
            particle.gameObject.SetActive(false);
            GetComponent<AudioSource>().PlayOneShot(breakSfx);
        }
        else if (damage < 2)
        {
            spriteRenderer.sprite = brokenSprites[damage];
        }
        damage++;
    }
}
