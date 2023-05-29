using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyLight : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool enabledOnAwake = true;

    [Header("Fade/Blink Settings")]
    [SerializeField] private float fadeDuration = 0f;
    [SerializeField] private float blinkWait = 0f;
    [SerializeField, Range(0.1f, 10f)] private float blinkOutMultiplier = 1f;

    private float _defaultIntensity;
    private bool _isFadingLight;
    private Light2D _light;

    private void Awake()
    {
        _light = GetComponentInChildren<Light2D>();
        _light.enabled = enabledOnAwake;
        _defaultIntensity = _light.intensity;
    }

    public void SetLightEnabled(bool enable, float customDuration = 0f)
    {
        if (_isFadingLight)
            return;

        if (fadeDuration == 0f && customDuration == 0f)
            _light.enabled = enable;
        else
            StartCoroutine(FadeLight(enable, customDuration));
    }

    //Use with animation events
    public void BlinkLight()
    {
        BlinkLight(enabledOnAwake);
    }

    public void BlinkLight(bool startEnabled, float customDuration = 0f)
    {
        if (_isFadingLight)
            return;
        
        StartCoroutine(Blink(startEnabled, customDuration, blinkWait));
    }

    private IEnumerator FadeLight(bool enable, float customDuration)
    {
        _isFadingLight = true;

        if (enable)
            _light.enabled = true;

        var elapsedTime = 0f;
        var duration = customDuration != 0f ? customDuration : fadeDuration;

        var startIntensity = _light.intensity;
        var finalIntensity = enable ? _defaultIntensity : 0f;

        while (elapsedTime < duration)
        {
            _light.intensity = Mathf.Lerp(startIntensity, finalIntensity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _light.intensity = finalIntensity;

        if (!enable)
            _light.enabled = false;

        _isFadingLight = false;
    }

    private IEnumerator Blink(bool startEnabled, float blinkDuration, float waitInterval)
    {
        var duration = blinkDuration != 0f ? blinkDuration : fadeDuration;

        StartCoroutine(FadeLight(!startEnabled, duration));

        while (_isFadingLight)
        {
            yield return null;
        }

        if (waitInterval != 0f)
            yield return new WaitForSeconds(waitInterval);
        
        StartCoroutine(FadeLight(startEnabled, duration * blinkOutMultiplier));
    }
}
