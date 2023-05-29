using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SpawnParticle : MonoBehaviour
{
    public float lifeTime = 2.5f;
    public float spawnTime = 2f;
	public GameObject spawnObject;
	public AudioClip spawnClip;

	public ParticleSystem darkParticle;
	public ParticleSystem lightParticle;
	public ParticleSystem sparkParticle;

	public AudioMixerGroup sfxGroup;

	public void SetupRewardSpawn(GameObject reward)
	{
		spawnObject = reward;
		reward.gameObject.SetActive(false);
		
		StartCoroutine(Spawn());
		Destroy(gameObject, 10);
	}

	public void SetupEnemySpawn(Enemy enemy)
	{
		this.spawnObject = enemy.gameObject;
		enemy.gameObject.SetActive(false);
		//SetColorOverLifeTime(enemy.primaryColor, enemy.primaryColor);
		SetParticleColorOverLifeTime(lightParticle, enemy.primaryColor);
		SetParticleColorOverLifeTime(darkParticle, enemy.secondaryColor);
		SetParticleColorOverLifeTime(sparkParticle, enemy.primaryColor);

		StartCoroutine(Spawn());
		Destroy(gameObject, 10);
	}

	IEnumerator Spawn()
	{
		yield return new WaitForSeconds(spawnTime);
		spawnObject.gameObject.SetActive(true);
		if (spawnClip)
		{
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();

			audioSource.outputAudioMixerGroup = sfxGroup;

			audioSource.PlayOneShot(spawnClip);
		}
	}

	private void SetColorOverLifeTime(Color colorA, Color colorB)
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		var col = particleSystem.colorOverLifetime;
		col.enabled = true;

		Gradient gradient = new Gradient();

		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(colorA, 1), new GradientColorKey(colorB, 0) },
			new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) } );
		col.color = gradient;
	}

	private void SetParticleColorOverLifeTime(ParticleSystem particle, Color color)
	{
		var col = particle.colorOverLifetime;
		col.enabled = true;

		Gradient gradient = new Gradient();

		Color lastColor = new Color(color.r, color.g, color.b, 0);

		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(color, 1), new GradientColorKey(lastColor, 0) },
			new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1) });
		col.color = gradient;
	}
}
