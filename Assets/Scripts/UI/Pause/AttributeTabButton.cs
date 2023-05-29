using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttributeTabButton : MonoBehaviour
{
	[SerializeField] Image buttonImage;
	[SerializeField] TextMeshProUGUI buttonText;

	[SerializeField] Sprite selectedSprite;
	[SerializeField] Sprite unselectedSprite;

    public void SetSelected(bool isSelected)
	{
		if (isSelected)
		{
			buttonImage.sprite = selectedSprite;
			buttonText.color = Color.white;
		}
		else
		{
			buttonImage.sprite = unselectedSprite;
			buttonText.color = Color.black;
		}
	}
}
