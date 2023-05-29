using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PocketPanel : MonoBehaviour
{
    Pocket currentPocket;
    Player player;

    [SerializeField] protected TextMeshProUGUI pocketName;
    [SerializeField] protected TextMeshProUGUI pocketSpecial;
    [SerializeField] protected TextMeshProUGUI pocketSpecialDescription;
    [SerializeField] protected TextMeshProUGUI pocketLevel;

    [SerializeField] protected Image pocketIcon;
    [SerializeField] protected AttributeBar pocketHpBar;
    [SerializeField] protected AttributeBar pocketSpecialStatsBar;
    [SerializeField] protected AttributeBar pocketDropRateBar;
    [SerializeField] protected AttributeBar pocketWeaponBar;
    [SerializeField] protected AttributeBar pocketHealingBar;
    [SerializeField] protected AttributeBar pocketMovementBar;

    public virtual void Setup(Pocket pocket)
    {
        currentPocket = pocket;

        if (!pocket) return;

        PocketDisplayInformations pocketInfo = pocket.GetComponent<PocketDisplayInformations>();
        pocketInfo.Setup();

        if (pocket.pocketType == PetType.Default)
        {
            pocketName.text = pocketInfo.pocketName;
            pocketSpecial.text = pocketInfo.pocketSpecialName;
            pocketSpecialDescription.text = pocketInfo.pocketDescription;
            pocketLevel.text = "Level " + pocket.level;

            pocketIcon.sprite = pocketInfo.icon;
            pocketIcon.rectTransform.localScale = new Vector3(1, pocketInfo.flipY ? -1 : 1, 1);

            pocketHpBar.SetValue(pocketInfo.GetAttributes().displayHp);
            pocketSpecialStatsBar.SetValue(pocketInfo.GetAttributes().specialStats);
            pocketDropRateBar.SetValue(pocketInfo.GetAttributes().dropRate);
            pocketWeaponBar.SetValue(pocketInfo.GetAttributes().weapon);
            pocketHealingBar.SetValue(pocketInfo.GetAttributes().healing);
            pocketMovementBar.SetValue(pocketInfo.GetAttributes().movement);
        }
    }

}
