using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseFreezingProjectile : Projectile
{
    [Header("Chase Target Settings")]
	[SerializeField] private Vector2 rotationSpeedRange = Vector2.zero;

    [Header("Freezing Settings")]
    [SerializeField] private float temperatureChange;

    private float _movementSpeed;
	private float _rotationSpeed;
    private Transform _target;

	[Header("Trail settings")]
	public TrailRenderer trail;
	public float scrollSpeed = 2;
	float trailoffset = 0;

	protected override void FixedUpdate()
	{
		if (_target)
		{
			Chase();
		}

		trailoffset += 1 * Time.deltaTime * scrollSpeed;
		trail.materials[0].mainTextureOffset = new Vector2(trailoffset, 0);
	}

	public override void Setup(Transform target, float damage, float speed, Transform shooterTransform)
	{
		this._target = target;
		this.damage = damage;
		_movementSpeed = speed;
		_rotationSpeed = Random.Range(rotationSpeedRange.x, rotationSpeedRange.y);
		this.shooterTransform = shooterTransform;

		ConfigureColorsAndAnimation();
	}

	void Chase()
	{
		if (rotationSpeedRange == Vector2.zero)
			Toolkit2D.RotateAt(spriteRenderer.transform, _target.position);
		else
			Toolkit2D.RotateAt(spriteRenderer.transform, _target, _rotationSpeed * Time.fixedDeltaTime);

		rb.velocity = spriteRenderer.transform.right * _movementSpeed;
	}

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root.TryGetComponent<Player>(out Player player))
        {
            if (player.GetHealth().isInvulnerable)
                return;

            player.ChangeTemperature(-temperatureChange);
        }

        base.OnTriggerEnter2D(other);
    }
}
