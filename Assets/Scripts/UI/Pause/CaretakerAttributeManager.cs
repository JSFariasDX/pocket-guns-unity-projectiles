using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaretakerAttributeManager : MonoBehaviour
{
    [SerializeField] CaretakerAttributesPanel caretakerAttributePanelPrefab;
    [SerializeField] Transform caretakerPanelOrganizer;
	List<CaretakerAttributesPanel> caretakerPanels = new List<CaretakerAttributesPanel>();

	public void CreatePanels()
	{
		ClearPanels();

		foreach (Player player in GameplayManager.Instance.GetPlayers(false))
		{
			CaretakerAttributesPanel panel = Instantiate(caretakerAttributePanelPrefab, caretakerPanelOrganizer);
			caretakerPanels.Add(panel);

			panel.Setup(player);
		}
	}

	public void ClearPanels()
	{
		foreach (CaretakerAttributesPanel panel in caretakerPanels) Destroy(panel.gameObject);
		caretakerPanels.Clear();
	}
}
