using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttributeBar : MonoBehaviour
{
    [SerializeField] List<Image> barTiles = new List<Image>();
	[SerializeField] Color defaultColor;
	[SerializeField] Color extraColor;

    public void SetValue(int value)
	{
		foreach(Image tile in barTiles) { tile.gameObject.SetActive(false); }

		for (int i = 0; i < value; i++) 
		{ 
			barTiles[i].gameObject.SetActive(true);
			barTiles[i].color = defaultColor;
		}
	}

	public void SetColorByInterval(Vector2Int interval, bool isExtra)
	{
		for (int i = interval.x; i < interval.y; i++) 
		{ 
			barTiles[i].gameObject.SetActive(true);
			barTiles[i].color = isExtra ? extraColor : defaultColor;
		}
	}
}
