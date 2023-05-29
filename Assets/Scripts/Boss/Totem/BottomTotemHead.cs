using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomTotemHead : TotemHead
{
    [SerializeField] List<Laser> rotateLasers = new List<Laser>();
	[SerializeField] List<Laser> lineLasers = new List<Laser>();

	private bool _rotatingLasersActive;
	private bool _lineLasersActive;

	private void Start()
	{
		ActiveFourLasers(false);
		ActiveLineLasers(false);
	}

	private void FixedUpdate()
	{
		if (IsAlive() && !IsRising())
		{
			if (totem.GetCurrentState() <= 2)
			{
				ActiveFourLasers(true);
			}
		}
	}

	public override void EnableAttacks()
	{
		base.EnableAttacks();
		ActiveFourLasers(true);
		StartCoroutine(Spin(.1f, false));
	}

	public void ActiveLineLasers(bool active)
	{
		if (_lineLasersActive)
			return;

		foreach (Laser laser in lineLasers)
		{
			laser.ActiveLaser(active);
		}

		_lineLasersActive = active;
	}

    public void ActiveFourLasers(bool active)
	{
		if (_rotatingLasersActive)
			return;

        foreach(Laser laser in rotateLasers)
		{
			laser.ActiveLaser(active);
		}

		_rotatingLasersActive = active;
	}

	public override void Die()
	{
		base.Die();

		foreach(Laser laser in rotateLasers)
		{
			laser.ActiveLaser(false);
		}

		foreach (Laser laser in lineLasers)
		{
			laser.ActiveLaser(false);
		}
	}
}
