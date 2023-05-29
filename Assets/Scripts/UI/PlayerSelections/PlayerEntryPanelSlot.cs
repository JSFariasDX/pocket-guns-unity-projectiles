using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntryPanelSlot : MonoBehaviour
{
	public GameObject background;
    PlayerEntryPanel panel;

	public PlayerEntryPanel GetPanel()
	{
		return panel;
	}

    public void SetPanel(PlayerEntryPanel panel)
	{
		this.panel = panel;
		panel.transform.SetParent(transform);
		panel.transform.localPosition = Vector2.zero;
		background.SetActive(false);
	}
}
