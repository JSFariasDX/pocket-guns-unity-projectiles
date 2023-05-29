using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
	public GameObject minimap;
	public GameObject loadingScreen;

	public void HideMinimap(bool hide)
	{
		minimap.SetActive(!hide);
	}
}
