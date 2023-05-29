using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeOut : MonoBehaviour
{
    public float startOpacity;
    public SpriteRenderer[] sprites;
    public float fadeTime;
    public float fadeCount;
    public float fadeStep;
    public float interval;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
	{
        interval = fadeTime / fadeCount;
        fadeStep = startOpacity / fadeCount;

        for (int i = 0; i <= fadeCount; i++)
		{
            yield return new WaitForSeconds(interval);
            float normalizedOpacity = (startOpacity - fadeStep * i) / 255;
            SetOpacity(normalizedOpacity);

            if (i == fadeCount)
			{
                Destroy(gameObject);
			}
		}
	}

    void SetOpacity(float opacity)
	{
        foreach(SpriteRenderer rend in sprites)
		{
            Color color = rend.color;
            color.a = opacity;
            rend.color = color;
		}
	}
}
