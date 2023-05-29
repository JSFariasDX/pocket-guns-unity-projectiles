using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLock : MonoBehaviour
{
    [SerializeField] int targetFps = 30;
	[SerializeField] bool lockFps;

	void Start()
	{
		if (lockFps)
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFps;
		}
	}
}
