using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PowerUpAttributesPanel : MonoBehaviour
{
    Player player;
    CollectibleDisplayInfo info;

    [SerializeField] Image powupIcon;
    [SerializeField] TextMeshProUGUI powerUpName;
    [SerializeField] TextMeshProUGUI powerUpDescription;

    public void Setup(Player player, CollectibleDisplayInfo info)
	{
        this.player = player;
        this.info = info;

        powupIcon.sprite = info.GetSprite();
        powerUpName.text = info.GetTitle();
        powerUpDescription.text = info.GetDescription();
	}

    public Player GetPlayer() 
    {
        return player;
    }

}
