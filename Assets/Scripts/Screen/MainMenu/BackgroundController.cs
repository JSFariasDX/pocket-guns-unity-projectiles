using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public Image backgroundImage;

    public float speed = 2;
    float value = 0;
    float desiredValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        value = Mathf.Lerp(value, desiredValue, speed * Time.deltaTime);

        backgroundImage.rectTransform.anchorMin = new Vector2(value, 0f);
        backgroundImage.rectTransform.anchorMax = new Vector2(value, 1f);
        backgroundImage.rectTransform.pivot = new Vector2(value, .5f);
    }

    public void SetMenu(bool open)
    {
        desiredValue = open ? 1 : 0;
    }
}
