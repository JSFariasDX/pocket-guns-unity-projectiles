using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketUpDownSelection : MonoBehaviour
{
    [Header("Pocket movement")]
    public AnimationCurve upDownCurve;
    public float animationSpeed;
    float curveTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveImage();
    }

    void MoveImage()
    {
        curveTime += Time.deltaTime * animationSpeed;

        transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, upDownCurve.Evaluate(curveTime), 0);

        if (curveTime > 1)
            curveTime = 0;
    }
}
