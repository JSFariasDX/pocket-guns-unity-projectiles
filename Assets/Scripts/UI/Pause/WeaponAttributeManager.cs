using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttributeManager : MonoBehaviour
{
	[SerializeField] WeaponAttributesPanel weaponAttributePanelPrefab;
	[SerializeField] Transform weaponPanelOrganizer;
	List<WeaponAttributesPanel> weaponPanels = new List<WeaponAttributesPanel>();

	public void CreatePanels()
	{
		ClearPanels();

		foreach (Player player in GameplayManager.Instance.GetPlayers(false))
		{
			WeaponAttributesPanel panel = Instantiate(weaponAttributePanelPrefab, weaponPanelOrganizer);
			weaponPanels.Add(panel);

			panel.Setup(player, 0);
		}
	}

	public void ClearPanels()
	{
		foreach (WeaponAttributesPanel panel in weaponPanels) Destroy(panel.gameObject);
		weaponPanels.Clear();
	}

	public List<WeaponAttributesPanel> GetPanels() { return weaponPanels; }

	public WeaponAttributesPanel GetPlayerPanel(Player player)
	{
		foreach(WeaponAttributesPanel panel in weaponPanels)
		{
			if (panel.GetPlayer() == player) return panel;
		}

		return null;
	}

}
