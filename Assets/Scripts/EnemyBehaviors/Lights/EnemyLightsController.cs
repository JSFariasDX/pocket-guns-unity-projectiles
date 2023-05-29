using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyLightsController : MonoBehaviour
{
    [SerializeField] bool playOnAwake = false;

    [Header("Components")]
    public Light2D controlledLight;

    [Header("Values")]
    public AnimationCurve lightCurve;
    float curveTimer = 0;

    public Vector2 minMaxIntensity;
    float intensity = 0;
    float desiredIntensity;
    public int loops = 3;
    int amountOfLoops;
    [Range(0, 5)] [Tooltip("Changes only work outside Play mode")]
    public float loopSpeed = 1;
    float actualSpeed;

    bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        controlledLight = GetComponentInChildren<Light2D>();

        controlledLight.intensity = minMaxIntensity.x;

        actualSpeed = Random.Range(loopSpeed - .1f, loopSpeed + .1f);

        if(playOnAwake)
            StartLoop();
    }

    // Update is called once per frame
    void Update()
    {
        intensity = Mathf.Clamp(intensity, 0, 3);
        intensity = Mathf.Lerp(0, 3, desiredIntensity);

        controlledLight.intensity = Mathf.Lerp(0, 3, desiredIntensity);

        LoopLight();
    }

    void LoopLight()
    {
        if (!started) return;
        if (amountOfLoops >= loops && loops > 0) return;

        if (curveTimer < 1)
            curveTimer += Time.deltaTime * actualSpeed;
        else
        {
            amountOfLoops++;
            curveTimer = 0;
        }

        desiredIntensity = Mathf.Lerp(minMaxIntensity.x, minMaxIntensity.y, lightCurve.Evaluate(curveTimer));
    }

    public void StartLoop()
    {
        ResetLoops();

        started = true;
    }

    void ResetLoops()
    {
        if (loops > 0)
            amountOfLoops = 0;
    }
}
