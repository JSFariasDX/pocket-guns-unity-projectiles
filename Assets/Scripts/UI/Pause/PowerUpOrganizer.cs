using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpOrganizer : MonoBehaviour
{
    Player player;
    int current;
    [SerializeField] RectTransform powerUpPanelParent;
    [SerializeField] ScrollRect scroll;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] GameObject powerupText;

    void SetScrollPosition(List<PowerUpAttributesPanel> panels, int target)
	{
        float interval = (float)1 / ((float)panels.Count - (float)1);
        float value = 1 - (float)interval * (float)target;

        scroll.verticalNormalizedPosition = value;
    }

    public void SetPowerupText(bool on)
    {
        powerupText.SetActive(on);
    }

    public void SetPlayer(Player player)
	{
        this.player = player;
	}

    public void Next(List<PowerUpAttributesPanel> panels)
	{
        if (panels.Count <= 0) return;

        if (current < panels.Count - 1)
            current++;

        SetScrollPosition(panels, current);
    }

    public void Previous(List<PowerUpAttributesPanel> panels)
	{
        if (panels.Count <= 0) return;

        if (current > 0)
            current--;

        SetScrollPosition(panels, current);
    }

    public Player GetPlayer() { return player; }

    public Transform GetPanelParent() { return powerUpPanelParent; }
}
