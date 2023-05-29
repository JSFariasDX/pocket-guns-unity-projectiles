using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPopUp : MonoBehaviour
{
    SpriteRenderer sprite;
    public float duration = 1;
    float alpha = 1;

    float timePassed;

    public bool start = false;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            timePassed += (Time.deltaTime / duration);

            sprite.transform.Translate(transform.up * (Time.deltaTime / duration));
            if(timePassed > duration / 2)
                alpha -= (Time.deltaTime / duration) * 2;
            sprite.color = new Color(1, 1, 1, alpha);
        }
    }

    public void Setup(Sprite icon, float duration = 1)
    {
        sprite.sprite = icon;
        this.duration = duration;

        Destroy(gameObject, duration);

        start = true;
    }
}
