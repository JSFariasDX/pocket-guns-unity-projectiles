using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
	private void Start()
	{
		transform.parent = FindObjectOfType<CinemachineTargetGroup>().transform;
		transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
	}
}
