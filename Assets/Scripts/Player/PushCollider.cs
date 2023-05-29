using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushCollider : MonoBehaviour
{
    public Transform target;

	private void Start()
	{
		transform.parent = null;
	}

	private void Update()
	{
		if (target != null) transform.position = target.transform.position;
	}
}
