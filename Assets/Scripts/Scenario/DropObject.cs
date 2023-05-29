using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField] private List<DropList> possibleDrops;

	public void Drop(Player p)
	{
		if (p == null)
			return;

		foreach (DropList dropList in possibleDrops)
		{
			var drop = dropList.EvaluateDrop(p);

			if (drop != null)
				SpawnDrop(drop);
		}
	}

	private void SpawnDrop(Drop dropToSpawn)
	{
		var randomObject = dropToSpawn.Prefabs[Random.Range(0, dropToSpawn.Prefabs.Count)];

		for (int i = 0; i < dropToSpawn.Count; i++) 
		{
			GameObject dropItself = Instantiate(randomObject, transform.position, Quaternion.identity);
			Rigidbody2D rb = dropItself.GetComponent<Rigidbody2D>();

			Vector2 force = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));

			rb.AddForce(force, ForceMode2D.Impulse);
		}
	}
}