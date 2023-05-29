using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaretakerAttributesPanel : MonoBehaviour
{
    Player player;
    CaretakerDisplayInfos caretakerInfos;

    [SerializeField] TextMeshProUGUI caretakerName;
    [SerializeField] TextMeshProUGUI caretakerDescription;

    [SerializeField] Image caretakerIcon;
    [SerializeField] AttributeBar caretakerHpBar;
    [SerializeField] AttributeBar caretakerSpeedBar;
    [SerializeField] AttributeBar caretakerDashCdBar;

    [SerializeField] Button resumeButton;
    [SerializeField] TextMeshProUGUI resumeTextButton;

	public void Setup(Player player)
	{
        this.player = player;
        caretakerInfos = player.GetComponent<CaretakerDisplayInfos>();
        caretakerInfos.Setup();

        caretakerName.text = caretakerInfos.caretakerName;
        caretakerDescription.text = caretakerInfos.description;

        caretakerIcon.sprite = caretakerInfos.icon;

        caretakerHpBar.SetValue(caretakerInfos.hp);
        caretakerSpeedBar.SetValue(caretakerInfos.speed);
        caretakerDashCdBar.SetValue(caretakerInfos.dashCooldown);
	}

    public void Setup(CaretakerDisplayInfos player)
    {
        caretakerInfos = player.GetComponent<CaretakerDisplayInfos>();

        caretakerName.text = caretakerInfos.caretakerName;
        caretakerDescription.text = caretakerInfos.description;

        caretakerIcon.sprite = caretakerInfos.icon;

        caretakerHpBar.SetValue(caretakerInfos.hp);
        caretakerSpeedBar.SetValue(caretakerInfos.speed);
        caretakerDashCdBar.SetValue(caretakerInfos.dashCooldown);
    }
}
