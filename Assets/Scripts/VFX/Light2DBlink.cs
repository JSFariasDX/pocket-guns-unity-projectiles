using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DBlink : MonoBehaviour
{
	public List<Light2D> lightComponents;

	public bool blinkOnStart;
	public bool offOnStart;
	public Vector2Int blinkTimesRange;
	public Vector2 blinkTimeInterval;
	public Vector2 turnOnTimeRange;
	public float minIntensityMultiplier = 0.6f;
	public float blinkIntensityMultiplier = 0.5f;
	public GameObject lightSprite;
	private bool isBlinking;

	float startIntensity;
	float minIntensity;

	private void Start()
	{
		if (blinkOnStart)
			StartBlink();
	}

	public void StartBlink()
	{
		if (lightComponents.Count == 0)
		{
			Light2D light = GetComponent<Light2D>();
			lightComponents.Add(light);
		}

		startIntensity = lightComponents[0].intensity;
		minIntensity = startIntensity * minIntensityMultiplier;

		if (lightSprite != null)
			lightSprite.SetActive(!offOnStart);

		StartCoroutine(Blink());
	}

	IEnumerator Blink()
	{
		foreach (Light2D light in lightComponents)
			light.intensity = offOnStart ? 0f : startIntensity;

		float turnOnTime = Random.Range(turnOnTimeRange.x, turnOnTimeRange.y);
		float interval = turnOnTime / 100;
		float intensityStep = (startIntensity - minIntensity) / 50;

		if (!offOnStart)
		{
			// decreasing intensity
			for (int i = 0; i < 50; i++)
			{
				yield return new WaitForSeconds(interval);
				foreach (Light2D light in lightComponents)
					light.intensity -= intensityStep;
			}
			for (int i = 0; i < 50; i++)
			{
				yield return new WaitForSeconds(interval);
				foreach (Light2D light in lightComponents)
					light.intensity += intensityStep;
			}
		}
		else
			yield return new WaitForSeconds(interval * 50f);

		//yield return new WaitForSeconds(turnOnTime);

		float blinkTimes = Random.Range(blinkTimesRange.x, blinkTimesRange.y);

		isBlinking = true;
		for (int i = 0; i < blinkTimes; i++)
		{
			foreach (Light2D light in lightComponents)
				light.intensity = startIntensity * blinkIntensityMultiplier;

			if (lightSprite != null)
			{
				bool blinkActive = offOnStart ? true : false;
				lightSprite.SetActive(blinkActive);
			}

			yield return new WaitForSeconds(Random.Range(blinkTimeInterval.x, blinkTimeInterval.y));

			foreach (Light2D light in lightComponents)
				light.intensity = offOnStart ? 0f: startIntensity;

			if (lightSprite != null)
			{
				bool blinkActive = offOnStart ? false : true;
				lightSprite.SetActive(blinkActive);
			}
				
			yield return new WaitForSeconds(Random.Range(blinkTimeInterval.x, blinkTimeInterval.y));
		}

		isBlinking = false;
		StartCoroutine(Blink());
	}
}
