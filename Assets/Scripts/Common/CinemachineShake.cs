using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    // https://www.youtube.com/watch?v=ACf1I27I6Tk
    public static CinemachineShake Instance { get; private set; }

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    float shakeTimer;
    float shakeTimerTotal;
    float startingIntensity;

    [Header("Debug")]
    [Range(-90, 90)]
    public float xValue;

    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        Instance = this;

        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
        startingIntensity = intensity;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
    }
}
