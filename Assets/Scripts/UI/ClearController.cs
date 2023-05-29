using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearController : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup clearCanvas;
    [SerializeField] float speed = 2;
    [SerializeField] float duration = 2;

    float value = 0;
    float desiredValue = 0;
    float desiredScale = 0;

    bool entering = false;

    // Start is called before the first frame update
    void Start()
    {
        //Invoke("Clear", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (entering)
        {
            desiredValue = Mathf.Lerp(desiredValue, 1, speed * Time.unscaledDeltaTime);
            desiredScale = Mathf.Lerp(desiredValue, 1, (speed / 2) * Time.unscaledDeltaTime);
        }
        else
        {
            desiredValue = Mathf.Lerp(desiredValue, 0, speed * Time.unscaledDeltaTime);
            desiredScale = Mathf.Lerp(desiredValue, 0, (speed / 2) * Time.unscaledDeltaTime);
        }

        if (entering)
            clearCanvas.GetComponent<RectTransform>().localScale = Vector2.Lerp(new Vector2(.9f, .9f), Vector2.one, desiredValue);

        clearCanvas.alpha = desiredValue;
    }

    public void Clear()
    {
        StopCoroutine(StartText());
        StartCoroutine(StartText());
    }

    IEnumerator StartText()
    {
        yield return new WaitForSeconds(.15f);

        value = 1;
        entering = true;

        yield return new WaitForSeconds(duration);

        value = 0;
        entering = false;
    }
}
