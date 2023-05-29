using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateOnDestroy : MonoBehaviour
{
	public GameObject prefab;
	Vector2 newPosition = Vector2.zero;

	private void OnDisable()
	{
		Instantiate(prefab, newPosition == Vector2.zero ? transform.position : newPosition, Quaternion.identity);
	}

	public void SetPosition(Vector2 position)
    {
		newPosition = position;
    }
}
