using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PowerUpAttributeManager : MonoBehaviour
{
	[SerializeField] PowerUpAttributesPanel powerUpAttributePanelPrefab;
	[SerializeField] Transform powerUpOrganizerParent;
	[SerializeField] PowerUpOrganizer powerUpPanelOrganizerPrefab;
	List<PowerUpOrganizer> powerUpOrganizers = new List<PowerUpOrganizer>();
	List<PowerUpAttributesPanel> powerUpPanels = new List<PowerUpAttributesPanel>();

	public List<PowerUpAttributesPanel> GetPanels() { return powerUpPanels; }

	public void CreatePanels()
	{
		ClearPanels();

		foreach (Player player in GameplayManager.Instance.GetPlayers(false))
		{
			PowerUpOrganizer powerUpOrganizer = Instantiate(powerUpPanelOrganizerPrefab, powerUpOrganizerParent);
			powerUpOrganizer.SetPlayer(player);
			powerUpOrganizers.Add(powerUpOrganizer);

			if (player.GetPowerUps().Count > 0)
			{
				powerUpOrganizer.SetPowerupText(false);

				foreach (CollectibleDisplayInfo info in player.GetPowerUps())
				{
					PowerUpAttributesPanel panel = Instantiate(powerUpAttributePanelPrefab, powerUpOrganizer.GetPanelParent());
					powerUpPanels.Add(panel);

					panel.Setup(player, info);
				}
            }
            else
            {
				powerUpOrganizer.SetPowerupText(true);
            }
		}
	}

	public PowerUpOrganizer GetPlayerOrganizer(Player player)
	{
		foreach (PowerUpOrganizer organizer in powerUpOrganizers)
		{
			print(organizer.GetPlayer());
			if (organizer.GetPlayer() == player) return organizer;
		}

		return null;
	}

	public void ClearPanels()
	{
		foreach (PowerUpAttributesPanel panel in powerUpPanels) Destroy(panel.gameObject);
		powerUpPanels.Clear();
		foreach (PowerUpOrganizer organizer in powerUpOrganizers) Destroy(organizer.gameObject);
		powerUpOrganizers.Clear();
	}
}
