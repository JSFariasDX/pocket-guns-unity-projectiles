using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticlesChanger : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private ParticleSystem mainParticles;
    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private ParticleSystem sparksParticles;

    [Header("Multiplier Settings")]
    [SerializeField] private int sparksCountMultiplier = 3;
    [SerializeField] private float mainRadiusMultiplier = 0.8f;
    [SerializeField] private float sparksRadiusMultiplier = 0.25f;
    [Header("Curve Settings")]
    [SerializeField] private AnimationCurve particleCountCurve;
    [SerializeField] private AnimationCurve shapeRadiusCurve;

    public void Setup(float splashDamage, float splashRadius, Gradient gradient)
    {
        var maxCount = particleCountCurve.Evaluate(splashDamage);
        SetMaxParticleCount((int)maxCount);

        var radius = shapeRadiusCurve.Evaluate(splashRadius);
        SetShapeRadius(radius);

        SetParticleColors(gradient);
    }

    private void SetParticleColors(Gradient gradient)
    {
        if (mainParticles != null)
        {
            var main = mainParticles.main;
            main.startColor = gradient.Evaluate(0f);
        }

        if (smokeParticles != null)
        {
            var smoke = smokeParticles.main;
            smoke.startColor = gradient.Evaluate(1f);
        }
    }

    private void SetMaxParticleCount(int maxCount)
    {
        if (mainParticles != null)
        {
            var main = mainParticles.main;
            main.maxParticles = maxCount;
        }

        if (smokeParticles != null)
        {
            var smoke = smokeParticles.main;
            smoke.maxParticles = maxCount;
        }

        if (sparksParticles != null)
        {
            var sparks = sparksParticles.main;
            sparks.maxParticles = maxCount * sparksCountMultiplier;
        }
    }

    private void SetShapeRadius(float radius)
    {
        if (mainParticles != null)
        {
            var main = mainParticles.shape;
            main.radius = radius * mainRadiusMultiplier;
        }

        if (smokeParticles != null)
        {
            var smoke = smokeParticles.shape;
            smoke.radius = radius;
        }

        if (sparksParticles != null)
        {
            var sparks = sparksParticles.shape;
            sparks.radius = radius * sparksRadiusMultiplier;
        }
    }
}
