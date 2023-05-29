using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Explodable))]
public class ExplodeOnClick : MonoBehaviour {

	private Explodable _explodable;

	void Start()
	{
		_explodable = GetComponent<Explodable>();
		//OnMouseDown();
	}
	void OnMouseDown()
	{
		_explodable.explode();
		ExplosionForce ef = GetComponent<ExplosionForce>();
			//GameObject.FindObjectOfType<ExplosionForce>();
		ef.doExplosion(transform.position);
	}
}
