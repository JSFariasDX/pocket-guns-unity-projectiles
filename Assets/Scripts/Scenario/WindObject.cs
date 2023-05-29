using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WindObject : MonoBehaviour
{
	public bool randomize;
	public WindType windType;
    [Range(0, 1)] public float weight;

	Rigidbody2D rig;
	WindManager windManager;

	private void Start()
	{
		rig = GetComponent<Rigidbody2D>();
		if (randomize) weight = weight * Random.Range(.8f, 1.2f);

		if (FindObjectOfType<WindManager>()) FindObjectOfType<WindManager>().AddWindObject(this);
	}

	public IEnumerator StartWind(Vector2 windDirection, float windForce, float windTime)
	{
		float windAdjustment = .00025f;
		float windInfluence = 1 - weight;

		float windInterval = .01f;
		float iterations = windTime / windInterval;

		for (int i = 0; i < iterations; i++)
		{
			if (windType == WindType.Rigidbody && rig) rig.AddForce(windDirection * windForce * windInfluence * 10, ForceMode2D.Force);
			else transform.Translate(windDirection * windForce * windAdjustment * windInfluence);
			
			yield return new WaitForSeconds(windInterval);
		}
	}

	public void ApplyWindForce(Vector2 windDirection, float windForce)
	{
		if (!enabled) return;

		float windAdjustment = .0025f;
		float windInfluence = 1 - weight;

		if (windType == WindType.Rigidbody && rig) rig.AddForce(windDirection * windForce * windInfluence * 10, ForceMode2D.Force);
		else transform.Translate(windDirection * windForce * windAdjustment * windInfluence);
	}

	private void OnDestroy()
	{
		try {
			FindObjectOfType<WindManager>().RemoveWindObject(this);
		}
		catch { }
	}
}

public enum WindType
{
	Translate, Rigidbody
}