using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PocketAttributeManager : MonoBehaviour
{
    [SerializeField] PocketAttributesPanel pocketAttributePanelPrefab;
    [SerializeField] Transform pocketPanelOrganizer;
	List<PocketAttributesPanel> pocketPanels = new List<PocketAttributesPanel>();

	public void CreatePanels()
	{
		ClearPanels();

		foreach (Player player in GameplayManager.Instance.GetPlayers(false))
		{
			PocketAttributesPanel panel = Instantiate(pocketAttributePanelPrefab, pocketPanelOrganizer);
			pocketPanels.Add(panel);

			panel.Setup(player, player.pockets[0]);
		}
	}

	public void ClearPanels()
	{
		foreach (PocketAttributesPanel panel in pocketPanels) Destroy(panel.gameObject);
		pocketPanels.Clear();
	}
	public PocketAttributesPanel GetPlayerPanel(Player player)
	{
		foreach (PocketAttributesPanel panel in pocketPanels)
		{
			if (panel.GetPlayer() == player) return panel;
		}

		return null;
	}

	public List<PocketAttributesPanel> GetPanels()
	{
		return pocketPanels;
	}

}
