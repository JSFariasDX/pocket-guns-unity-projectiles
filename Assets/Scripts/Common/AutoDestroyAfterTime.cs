using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AutoDestroyAfterTime : MonoBehaviour
{
    [SerializeField] private bool autoDestroyOnStart = true;
    public float destroyTime = 3f;
    DateTime spawnedTime;
    public bool flashBeforeDestroy = false;
    public float flashAfterTime = 1.5f;
    bool isFlashing = false;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (!autoDestroyOnStart)
            return;
        
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, destroyTime);
		spawnedTime = System.DateTime.Now;
    }

    public void SetAutoDestroy(float multiplier = 1f)
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, destroyTime * multiplier);
		spawnedTime = System.DateTime.Now;
    }

    private void Update()
    {
        if (flashBeforeDestroy)
        {
            DateTime livingTime = System.DateTime.Now;
            double difference = (livingTime - spawnedTime).TotalSeconds;
            if (difference > flashAfterTime && !isFlashing)
            {
                StartCoroutine("Flash");
            }
        }

    }

    IEnumerator Flash()
    {
        if (spriteRenderer)
        {
            isFlashing = true;
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = new Color(1f, 1f, 1f, .5f);
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            isFlashing = false;
        } 
    }
}
