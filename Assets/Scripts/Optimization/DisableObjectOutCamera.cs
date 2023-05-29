using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectOutCamera : MonoBehaviour
{
	[SerializeField] GameObject target;

	private void OnBecameVisible()
	{
		target.SetActive(true);
	}

	private void OnBecameInvisible()
	{
		target.SetActive(false);
	}
}
