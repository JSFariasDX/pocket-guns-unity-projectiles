using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
	public Vector2 windForceRange;
    public Vector2 windDurationRange;
    public Vector2 windIntervalRange;

	private float windDuration;
	private float windForce;
	private Vector2 windDirection;

	public WindParticle windParticlePrefab;
	AudioSource audioSource;
	private List<WindObject> windObjects = new List<WindObject>();
	public Coroutine windRoutine;

	bool isWinding = false;

	WindParticle particle;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

    [System.Obsolete]
    private void FixedUpdate()
	{
		if (PauseManager.Instance.IsGamePaused())
		{
			if (audioSource.isPlaying) audioSource.Pause();
		}
		else
		{
			if (!audioSource.isPlaying) audioSource.UnPause();
		}
		if (GameplayManager.Instance.currentDungeonType == DungeonType.Forest && !isWinding)
		{
			windRoutine = StartCoroutine("StartWind");
		} else if (GameplayManager.Instance.currentDungeonType != DungeonType.Forest && windRoutine != null)
        {
			if (!isWinding)
				return;
				
			StopCoroutine(windRoutine);
			finishWind = true;
			EndWindEffects();
			windRoutine = null;
        }
	}

    private Vector2 GetRandomWindDirection()
	{
		float rand = Random.value;
		Vector2 dir = Vector2.zero;

		// Vertical
		if (rand < .5f)
		{
			dir.x = 1;
			if (Random.value < .5f) dir.x = -1;
		}
		// Horizontal
		else
		{
			dir.y = 1;
			if (Random.value < .5f) dir.y = -1;
		}
		
		return dir;
	}

	private void SpawnWindParticle(Vector2 position, float duration, float angle)
	{
		particle = Instantiate(windParticlePrefab, Camera.main.transform);
		particle.transform.position = position;
		duration -= 2;
		particle.Setup(duration);
		
		Vector3 angles = particle.transform.eulerAngles;
		angles.z = angle;
		particle.transform.eulerAngles = angles;
	}

	bool finishWind;
	[System.Obsolete]
	public IEnumerator StartWind()
	{
		isWinding = true;

		float windInterval = Random.Range(windIntervalRange.x, windIntervalRange.y);
		yield return new WaitForSeconds(windInterval);

		// Start wind settings
		windDuration = 7;
		windDirection = GetRandomWindDirection();
		StartCoroutine(FinishWind(windDuration));

		// Spawn particle
		
		if (windDirection.x > 0)
		{
			SpawnWindParticle(Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2)), windDuration + 1f, -90);
		}
		else if (windDirection.x < 0)
		{
			SpawnWindParticle(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2)), windDuration, 90);
		}
		else if (windDirection.y > 0)
		{
			SpawnWindParticle(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0)), windDuration + .75f, 0);
		}
		else
		{
			SpawnWindParticle(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height)), windDuration + .75f, 180);
		}

		// Apply wind force
		windForce = Random.Range(windForceRange.x, windForceRange.y);

		float iterationInterval = .01f;
		float iterations = windDuration / iterationInterval - 1;

		//for (int i = 0; i < iterations; i++)
		while(!finishWind)
		{
			foreach (WindObject windObject in windObjects)
			{
				if (windObject.enabled)
				{
					windObject.ApplyWindForce(windDirection, windForce);
				}
			}

			yield return new WaitForSeconds(iterationInterval);
		}

		// Finish wind
		EndWindEffects();
	}

    [System.Obsolete]
    public void EndWindEffects()
    {
		isWinding = false;
		if (!particle)
        {
			return;
        }
		particle.GetComponent<ParticleSystem>().enableEmission = false;
		Destroy(particle.GetComponent<AudioSource>());
		Destroy(particle.gameObject, 5f);
		finishWind = false;
	}

	public void AddWindObject(WindObject windObject)
	{
		windObjects.Add(windObject);
	}

	public void RemoveWindObject(WindObject windObject)
	{
		windObjects.Remove(windObject);
	}

	IEnumerator FinishWind(float duration)
	{
		yield return new WaitForSeconds(duration);
		finishWind = true;
	}
}
