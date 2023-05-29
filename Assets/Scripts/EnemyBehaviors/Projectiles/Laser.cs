using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Laser : MonoBehaviour
{
	[Header("General Settings")]
	[SerializeField] private bool changeSortingLayer = true;
	[SerializeField] float damage;
	[SerializeField] float rotateSpeed;
	[SerializeField] Direction laserDirection;
	[SerializeField] LayerMask laserHitLayers;

	[Header("VFX Settings")]
	[SerializeField] GameObject laserStartParticle;
	[SerializeField] GameObject laserEndParticle;
	[SerializeField] private bool changeParticleRotation = false;
	[SerializeField] private LineRenderer whiteLine;
	[SerializeField] private Light2D light2D;
	[SerializeField, Range(0f, 3f)] private float lightSizeMultiplier = 1f;

	LineRenderer lineRenderer;
	private ParticleSystem _startParticleSystem;
	private ParticleSystem _endParticleSystem;

	public void ActiveLaser(bool active)
	{
		gameObject.SetActive(active);

		if (active)
		{
			_endParticleSystem.Play();
			if (laserStartParticle != null)
				_startParticleSystem.Play();
		}
		else
		{
			_endParticleSystem.Stop();
			if (laserStartParticle != null)
				_startParticleSystem.Stop();
		}
	}

	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		_endParticleSystem = laserEndParticle.GetComponentInChildren<ParticleSystem>();

		if (laserStartParticle != null)
			_startParticleSystem = laserStartParticle.GetComponentInChildren<ParticleSystem>();
	}

	private void FixedUpdate()
	{
		transform.Rotate(0, 0, rotateSpeed * Time.fixedDeltaTime);
		DrawLaser();
	}

	void DrawLaser()
	{
		Vector2 startPoint = transform.position + transform.up * .5f;
		Vector2 endPoint = Physics2D.Raycast(transform.position, transform.up, 99, laserHitLayers).point;

		SetLinePositions(lineRenderer, startPoint, endPoint);
		SetLinePositions(whiteLine, startPoint, endPoint);

		SetLightSize(Vector2.Distance(startPoint, endPoint));

		laserEndParticle.transform.position = endPoint;

		if (_startParticleSystem != null)
			laserStartParticle.transform.position = startPoint;

		if (changeParticleRotation)
		{
			laserStartParticle.transform.rotation = transform.rotation;
			laserEndParticle.transform.rotation = transform.rotation;
		}

		HandleLayers();
	}

	private void HandleLayers()
	{
		if (!changeSortingLayer)
			return;

		if (transform.eulerAngles.z > 360) transform.eulerAngles = Vector3.zero;
		
		lineRenderer.sortingOrder = transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270 ? 7 : 4;
		whiteLine.sortingOrder = transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270 ? 8 : 5;
		
		if (_startParticleSystem == null)
			return;

		if (transform.eulerAngles.z > 90 && transform.eulerAngles.z < 270)
			_startParticleSystem.Play();
		else
			_startParticleSystem.Stop();
	}

	private void SetLinePositions(LineRenderer line, Vector2 startPoint, Vector2 endPoint)
	{
		line.SetPosition(0, startPoint);
		line.SetPosition(1, endPoint);
	}

	private void SetLightSize(float size)
	{
		light2D.transform.localScale = new Vector3(light2D.transform.localScale.x, size * lightSizeMultiplier, light2D.transform.localScale.z);
	}

	public float GetDamage() { return damage; }
}
