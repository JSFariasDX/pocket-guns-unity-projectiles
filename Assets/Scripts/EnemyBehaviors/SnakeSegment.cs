using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment : MonoBehaviour
{
    public SnakeSegment Parent { get; set; }
	public SnakeSegment Child { get; set; }

	[SerializeField] protected SpriteRenderer sprite;

	protected virtual void Start()
	{
		if (GameplayManager.Instance != null)
			GameplayManager.Instance.clearOnDungeonEnd.Add(gameObject);
	}

	protected virtual void FixedUpdate()
	{
		if (Parent)
		{
			Toolkit2D.RotateAt(sprite.transform, Parent.transform);
		}
	}

	public void SetupJoint(Rigidbody2D rig)
	{
		GetComponent<Joint2D>().connectedBody = rig;
	}

	private void OnHitted()
	{
	}
}
