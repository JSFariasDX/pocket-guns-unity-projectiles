using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPulse : MonoBehaviour
{
    [SerializeField]
    Light2D controlledLight;

    [SerializeField]
    public float minimum = -1.0F;
    [SerializeField]
    public float maximum = 1.0F;

    [SerializeField]
    float speed = 2f;

    float target;

    private void Start()
    {
        target = maximum;
    }

    private void Update()
    {
        float currentValue = controlledLight.intensity;
        float delta = speed * Time.deltaTime;
        controlledLight.intensity = Mathf.MoveTowards(currentValue, target, delta);
        if (currentValue >= maximum)
        {
            target = minimum;
        } else if (currentValue <= minimum)
        {
            target = maximum;
        }
    }

}
