using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFilterManager : MonoBehaviour
{
    [SerializeField] GameObject characterHitFilter;
    [SerializeField] GameObject pocketHitFilter;

	private void Start()
	{
		ActiveCharacterHitFilter(false);
		ActivePocketHitFilter(false);
	}

	// Character
	public void ActiveCharacterHitFilter(bool value)
	{
		characterHitFilter.SetActive(value);
	}
	
    public void ActiveCharacterHitFilter(float time)
	{
		StartCoroutine(CharacterHitFilter(time));
	}

	IEnumerator CharacterHitFilter(float time)
	{
		characterHitFilter.SetActive(true);
		yield return new WaitForSeconds(time);
		characterHitFilter.SetActive(false);
	}

	// Pocket
	public void ActivePocketHitFilter(bool value)
	{
		pocketHitFilter.SetActive(value);
	}

	public void ActivePocketHitFilter(float time)
	{
		StartCoroutine(PocketHitFilter(time));
	}

	IEnumerator PocketHitFilter(float time)
	{
		pocketHitFilter.SetActive(true);
		yield return new WaitForSeconds(time);
		pocketHitFilter.SetActive(false);
	}
}
