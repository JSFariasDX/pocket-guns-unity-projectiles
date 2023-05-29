using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiAnimator : MonoBehaviour
{
	public UiSpriteAnimationData animationData;
	public Image targetImage;

	private int currentSprite;

	private bool _stopCoroutine;

	private void OnEnable()
	{
		targetImage = GetComponent<Image>();
		StartCoroutine(NextFrame());
		_stopCoroutine = false;
	}

	public void StopAnimation()
	{
		_stopCoroutine = true;
	}

	public IEnumerator NextFrame()
	{
		while (true)
		{
			currentSprite++;
			if (currentSprite >= animationData.sprites.Length)
			{
				currentSprite = 0;
			}
			targetImage.sprite = animationData.sprites[currentSprite];

			if (_stopCoroutine)
				yield break;

			yield return new WaitForSecondsRealtime(animationData.frameSpeed);
		}
	}
}
