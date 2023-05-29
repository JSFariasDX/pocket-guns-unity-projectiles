using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterClawVfx : MonoBehaviour
{
    public static bool isPlaying;

    void Start()
    {
        if (isPlaying)
		{
            GetComponent<AudioSource>().volume = 0;
        }        
        isPlaying = true;
    }

	private void OnDestroy()
	{
        isPlaying = false;
	}
}
