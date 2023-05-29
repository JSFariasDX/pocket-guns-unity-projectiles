using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
	private Image image;
    public Sprite[] sprites;
    public float animationSpeed = .15f;

    private int currentSprite;

	private void OnEnable()
	{
		image = GetComponent<Image>();
		StartCoroutine(NextFrame());
	}

	public IEnumerator NextFrame()
	{
		currentSprite++;
		if (currentSprite >= sprites.Length)
		{
			currentSprite = 0;
		}
		image.sprite = sprites[currentSprite];

		yield return new WaitForSeconds(animationSpeed);

		StartCoroutine(NextFrame());
	}
}