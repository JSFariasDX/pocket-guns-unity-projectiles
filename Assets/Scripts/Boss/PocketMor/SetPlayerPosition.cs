using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerPosition : MonoBehaviour
{
	public void MovePlayers()
    {
        foreach(Player player in FindObjectsOfType<Player>())
		{
            player.transform.position = transform.position;
		}
    }
}
