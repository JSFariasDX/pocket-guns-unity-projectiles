using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingUI : MonoBehaviour
{
    [Header("Parameters")]
    public AnimationCurve alphaCurve;
    float curveTime = 0;
    [Range(0, 6)] 
    public float curveSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (curveTime < 1)
            curveTime += Time.deltaTime * curveSpeed;
        else
            curveTime = 0;

        GetComponent<CanvasGroup>().alpha = alphaCurve.Evaluate(curveTime);
    }
}
