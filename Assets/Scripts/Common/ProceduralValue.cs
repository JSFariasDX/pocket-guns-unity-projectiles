using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProceduralValue
{
	public float min;
	public float max;
	public float superMin;
	public float superMax;
	[Tooltip("Examples: 1 for 100% and 0.5 for 50%")] public float superMinChanceRate = .05f;
	[Tooltip("Examples: 1 for 100% and 0.5 for 50%")] public float superMaxChanceRate = .05f;

	public int GetRandomIntegerValue()
	{
		float rand = Random.value;

		if (rand < superMinChanceRate) return (int)superMin;
		else if (rand > 1 - superMaxChanceRate) return (int)superMax;
		else return (int)Random.Range(min, max + 1);
	}

	public float GetRandomFloatValue()
	{
		float rand = Random.value;

		if (rand < superMinChanceRate) return superMin;
		else if (rand > 1 - superMaxChanceRate) return superMax;
		else return Random.Range(min, max);
	}
}
