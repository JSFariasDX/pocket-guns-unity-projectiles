using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorProjectile : Projectile
{
	[Header("Meteor Settings")]
	[SerializeField] bool fallOnPlayerPosition;
	[SerializeField] float upTime;
    [SerializeField] GameObject shadow;
	[SerializeField] GameObject explosionPrefab;
	[SerializeField] float projectileUpSpeed;
	[SerializeField] float projectileDownSpeed;

	float timer;
    Vector3 targetPosition;
	bool isFalling;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void FixedUpdate()
	{
		timer += Time.fixedDeltaTime;
		if (timer > upTime && !isFalling)
		{
			isFalling = true;

			if (fallOnPlayerPosition)
				targetPosition = shooterTransform.GetComponent<Enemy>().GetRandomPlayer().transform.position;

			transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
		}
		
		// Movement
		if (isFalling)
		{
			transform.Translate(0, projectileDownSpeed * Time.fixedDeltaTime, 0);

			if (transform.position.y <= targetPosition.y)
			{
				OnTrigger();
			}
			else if (transform.position.y <= targetPosition.y + 1)
			{
				GetComponent<Collider2D>().enabled = true;
			}
			else if (transform.position.y <= targetPosition.y + 12)
			{
				shadow.transform.position = targetPosition;
				shadow.gameObject.SetActive(true);
			}
		}
		else
		{
			transform.Translate(0, projectileUpSpeed * Time.fixedDeltaTime, 0);
		}
	}

	public override void OnTrigger()
	{
		//Explode();
		base.OnTrigger();
	}

	private void OnDestroy()
	{
		Destroy(shadow);
	}

	void Explode()
	{
		GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		FindObjectOfType<ScreenShakeTrigger>().Shake(.7f, .2f);
	}

	public override void Setup(Vector3 targetPosition, float damage, float speed, Transform shooterTransform)
	{
		if (!fallOnPlayerPosition) 
			this.targetPosition = targetPosition;
		this.damage = damage;
		this.shooterTransform = shooterTransform;

		shadow.transform.parent = null;
		shadow.gameObject.SetActive(false);
		shadow.transform.position = targetPosition;
		GetComponent<Collider2D>().enabled = false;

        ConfigureColorsAndAnimation();
	}
}
