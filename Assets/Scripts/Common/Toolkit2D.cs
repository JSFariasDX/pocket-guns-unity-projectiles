using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Toolkit2D
{
	/// <summary>
	/// Switch the SpriteRenderer between enabled true and false
	/// </summary>
	/// <param name="renderer">The sprite that will be blinked</param>
	/// <param name="blinkTime">The blink total time</param>
	/// <param name="blinkInterval">The interval between each blink</param>
	/// <returns></returns>
	public static IEnumerator SpriteRendererBlink(SpriteRenderer renderer, float blinkTime, float blinkInterval)
	{
		int blinkTimes = (int)(blinkTime / blinkInterval);
		renderer.enabled = false;

		for (int i = 0; i < blinkTimes; i++)
		{
			yield return new WaitForSeconds(blinkInterval);
			renderer.enabled = !renderer.enabled;
		}

		renderer.enabled = true;
	}

	/// <summary>
	/// Make an object look at a target
	/// </summary>
	/// <param name="obj">The object that gonna look at</param>
	/// <param name="target">The target object</param>
	public static void RotateAt(Transform obj, Transform target)
	{
		var angle = GetAngle(obj, target.transform.position);
		obj.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	public static void RotateAt(Transform obj, Vector3 target)
	{
		var angle = GetAngle(obj, target);
		obj.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	public static void RotateAt(Transform obj, Transform target, float speed)
	{
		var angle = GetAngle(obj, target.transform.position);

		var angleDirection = Quaternion.Euler(0f, 0f, angle);
		var newRotation = Quaternion.RotateTowards(obj.rotation, angleDirection, speed);
		obj.rotation = newRotation;
	}

	private static float GetAngle(Transform obj, Vector3 target)
	{
		target.z = 0f;

		Vector3 objectPos = obj.position;
		target.x = target.x - objectPos.x;
		target.y = target.y - objectPos.y;

		float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
		return angle;
	}

	public static float GetAngleBetweenTwoPoints(Vector2 origin, Vector2 target)
	{
		Vector2 v2 = (target - origin).normalized;
		float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
		if (angle < 0)
			angle = 360 + angle;
		angle = 360 - angle;

		return angle + 90;
	}

	/// <summary>
	/// Return a random outside of camera position
	/// </summary>
	/// <returns></returns>
	public static Vector2 GetRandomOutsideCameraPosition(float maxCameraDistance)
	{
		Camera cam = Camera.main;

		Vector2 pos = Vector2.zero;

		Vector2 camTopRight = cam.ViewportToWorldPoint(new Vector2(1.1f, 1.1f));
		Vector2 camBottomLeft = cam.ViewportToWorldPoint(new Vector2(-0.1f, -0.1f));

		float rand = Random.value;
		if (rand < .25f)
		{ // Top
			pos.x = Random.Range(camBottomLeft.x, camTopRight.x);
			pos.y = camTopRight.y + Random.Range(0, maxCameraDistance);
		}
		else if (rand < .5f)
		{ // Right
			pos.x = camTopRight.x + Random.Range(0, maxCameraDistance);
			pos.y = Random.Range(camBottomLeft.y, camTopRight.y);
		}
		else if (rand < .75f)
		{ // Bottom
			pos.x = Random.Range(camBottomLeft.x, camTopRight.x);
			pos.y = camBottomLeft.y - Random.Range(0, maxCameraDistance);
		}
		else
		{ // Left
			pos.x = camBottomLeft.x - Random.Range(0, maxCameraDistance);
			pos.y = Random.Range(camBottomLeft.y, camTopRight.y);
		}

		return pos;
	}
}