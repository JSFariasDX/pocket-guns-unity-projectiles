using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCorridorSfx : MonoBehaviour
{
	List<GameObject> listeningPlayers = new List<GameObject>();
	static AudioSource audioSource;

	private void Start()
	{
		if (audioSource) Destroy(audioSource);
		audioSource = GetComponent<AudioSource>(); 
	}

	private void Update()
	{
		if (FindObjectOfType<Portal>()) Destroy(audioSource);

		if (listeningPlayers.Count > 0 && audioSource.volume < 1)
		{
			audioSource.volume += Time.deltaTime;
		}
		else if (audioSource.volume > 0)
		{
			audioSource.volume -= Time.deltaTime;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player")) listeningPlayers.Add(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player")) listeningPlayers.Remove(other.gameObject);
	}
}
