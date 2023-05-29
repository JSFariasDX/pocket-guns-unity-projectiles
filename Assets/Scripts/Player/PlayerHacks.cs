using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHacks : MonoBehaviour
{
    public void BuffGuns()
	{
		foreach(Gun gun in FindObjectOfType<Player>().currentGuns)
		{
			gun.damage = 10000;
		}
	}
}
