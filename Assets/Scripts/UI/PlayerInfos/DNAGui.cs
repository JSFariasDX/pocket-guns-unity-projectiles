using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DNAGui : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DNAText;
    RectTransform thisTransform;
    [SerializeField] float scaleSpeed = 2;
    float actualScale = 0;

    [Header("Add UI")]
    [SerializeField] TextMeshProUGUI addText;
    float addAmount;
    float amountToAdd;
    float desiredAmount;
    [SerializeField] float speed = 50;
    bool canDecrease = false;

    float DNAAmount;
    float desiredDNAAmount;

    private void Start()
    {
        thisTransform = GetComponent<RectTransform>();

        thisTransform.localScale = new Vector3(1, 0, 1);
    }

    void Update()
    {
        DNAText.text = desiredDNAAmount.ToString("000");
        thisTransform.localScale = new Vector3(1, Mathf.Lerp(thisTransform.localScale.y, actualScale, scaleSpeed * Time.unscaledDeltaTime), 1);

        if (thisTransform.localScale.y < .05f) GetComponent<CanvasGroup>().alpha = 0;
        else GetComponent<CanvasGroup>().alpha = 1;

        addText.text = "+" + desiredAmount.ToString("0");

        if (canDecrease)
        {
            desiredAmount = Mathf.MoveTowards(desiredAmount, addAmount, speed * Time.unscaledDeltaTime);
            desiredDNAAmount = Mathf.MoveTowards(desiredDNAAmount, DNAAmount + amountToAdd, speed * Time.unscaledDeltaTime);
        }
    }

    public void OpenCounter(float time, int value)
    {
        StopAllCoroutines();
        StartCoroutine(CounterValue(time, value));
    }

    IEnumerator CounterValue(float interval, int value)
    {
        addAmount += value;
        desiredAmount = addAmount;
        DNAAmount = PlayerPrefs.GetInt("DNA", 0) - addAmount;
        desiredDNAAmount = DNAAmount;
        print("<color=cyan>" + PlayerPrefs.GetInt("DNA", 0) + "</color>");
        canDecrease = false;

        yield return new WaitForSecondsRealtime(.1f);

        actualScale = 1;

        yield return new WaitForSecondsRealtime(1);
        amountToAdd = addAmount;
        addAmount = 0;
        canDecrease = true;

        yield return new WaitUntil(() => desiredAmount <= 0);

        yield return new WaitForSecondsRealtime(interval);

        actualScale = 0;
    }
}
