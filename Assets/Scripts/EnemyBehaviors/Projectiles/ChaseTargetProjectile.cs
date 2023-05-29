using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTargetProjectile : Projectile
{
	[Header("Chase Target Settings")]
	[SerializeField] private float rotationSpeed = 60;

    private float _movementSpeed;
	private float _rotationSpeed;
    private Transform _target;
	public bool canChase = false;
	float targetDistance;

	[Header("Trail settings")]
	public TrailRenderer trail;
	public float scrollSpeed = 2;
	float trailoffset = 0;

    private void Start()
    {
		StartCoroutine(SetCanChase());
    }

    protected override void FixedUpdate()
	{
		if (_target)
		{
			targetDistance = Vector2.Distance(_target.position, transform.position);
			Chase();

			if (targetDistance < .5f)
				Destroy(gameObject);
		}
		else
			NoChase();

        

        if (trail)
		{
			trailoffset += 1 * Time.deltaTime * scrollSpeed;
			trail.materials[0].mainTextureOffset = new Vector2(trailoffset, 0);
		}
	}

	public override void Setup(Transform target, float damage, float speed, Transform shooterTransform)
	{
		this._target = target;
		this.damage = damage;
		_movementSpeed = speed;
		_rotationSpeed = rotationSpeed;
		this.shooterTransform = shooterTransform;

		ConfigureColorsAndAnimation();
	}

	void Chase()
	{
		_movementSpeed += Time.deltaTime;
		_rotationSpeed += Time.deltaTime * 50;

        Toolkit2D.RotateAt(transform, _target, _rotationSpeed * Time.fixedDeltaTime);

        rb.velocity = transform.right * _movementSpeed;
    }

	void NoChase()
    {
		rb.velocity = transform.right * _movementSpeed;
	}

    private void OnDestroy()
    {
		OnTrigger();
    }

    public IEnumerator SetCanChase()
    {
		canChase = false;

		yield return new WaitForSeconds(.5f);

		canChase = true;
    }
}
