using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NPCPocketEvolver : NPCActiveCanvasGroup
{
	[SerializeField] Pocket currentPocket;

    public void Evolve()
	{
		HideCanvas();
		currentPocket.Evolve();
	}
	
	public void SetCurrentPocket(Pocket pocket)
	{
		currentPocket = pocket;
	}
}
