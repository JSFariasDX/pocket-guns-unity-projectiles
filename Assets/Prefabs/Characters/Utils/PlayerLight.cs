using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
	Light2D light;

	private void Start()
	{
		light = GetComponent<Light2D>();
	}

	public void SetActive(bool active)
	{
		light.enabled = active;
	}
}
