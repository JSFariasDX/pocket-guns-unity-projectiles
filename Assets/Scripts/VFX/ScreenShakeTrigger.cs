using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShakeTrigger : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;
    public bool onStart;
    [SerializeField] private float intensity = 2f;
    [SerializeField] private float duration = 0.3f;

	private void Start()
	{
		if (onStart)
		{
			Shake();
		}
	}

	public void Shake()
    {
        CinemachineShake.Instance.ShakeCamera(intensity, duration);
    }

    public void Shake(float intensity, float duration)
    {
        CinemachineShake.Instance.ShakeCamera(intensity, duration);
    }
}
