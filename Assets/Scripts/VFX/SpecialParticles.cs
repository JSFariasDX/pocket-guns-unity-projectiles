using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpecialParticles : MonoBehaviour
{
    [Header("Config")]
    public bool needsSetup = true;
    public bool followPlayerDirection = false;
    public Transform target;
    public float duration;

    [Header("Particles")]
    public ParticleSystem mainParticle;
    public ParticleSystem lineParticle;

    [Header("Light")]
    public Light2D healLight;
    public AnimationCurve blinkCurve;
    float curveTime;

    Vector2 direction;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 5;

    // Start is called before the first frame update
    void Start()
    {
        if(target)
            Setup(target, duration);
    }

    // Update is called once per frame
    void Update()
    {
        Light();

        Follow();

        if (followPlayerDirection)
            InvertPlayerDirection();
    }

    void Light()
    {
        if (!healLight) return;

        curveTime += Time.deltaTime / mainParticle.main.duration;

        healLight.intensity = blinkCurve.Evaluate(curveTime);
    }

    void Follow()
    {
        if (target)
            transform.position = target.position;
    }

    void InvertPlayerDirection()
    {
        direction = target.GetComponent<Player>().movement;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void Setup(Transform target, float duration)
    {
        this.target = target;

        mainParticle.Stop();

        if (lineParticle)
            lineParticle.Stop();

        if (needsSetup)
        {
            this.duration = duration;

            var mainModule = mainParticle.main;
            var linesMainModule = lineParticle ? lineParticle.main : mainParticle.main;

            mainModule.duration = this.duration;
            linesMainModule.duration = this.duration;
        }

        mainParticle.Play();

        if (lineParticle)
            lineParticle.Play();
    }
}
