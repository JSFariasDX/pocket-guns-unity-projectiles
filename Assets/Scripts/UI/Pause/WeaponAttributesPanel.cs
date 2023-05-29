using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponAttributesPanel : MonoBehaviour
{
    Player player;
    Gun gun;
    int currentIndex;

    [SerializeField] Image weaponIcon;
    [SerializeField] TextMeshProUGUI weaponName;

    [SerializeField] AttributeBar damageBar;
    [SerializeField] AttributeBar ammoBar;
    [SerializeField] AttributeBar fireRateBar;
    [SerializeField] AttributeBar rechargeBar;

    [SerializeField] GameObject arrow;

    public void Setup(Player player, int currentIndex)
	{
        this.player = player;
        this.currentIndex = currentIndex;

        gun = player.currentGuns[currentIndex];

        weaponIcon.sprite = gun.gunConfig.sideView;
        weaponName.text = gun.gunConfig.GetLocalizedName();

        damageBar.SetValue(gun.gunConfig.damageInfo);
        ammoBar.SetValue(gun.gunConfig.ammoInfo);
        fireRateBar.SetValue(gun.gunConfig.fireRateInfo);
        rechargeBar.SetValue(gun.gunConfig.rechargeInfo);

        arrow.SetActive(player.currentGuns.Count > 1);
    }

    public void Next()
    {
        print("NEXT WEAPON");
        Setup(player, player.GetNextGunIndex(currentIndex));
    }

    public void Previous()
    {
        print("PREVIOUS WEAPON");
        Setup(player, player.GetPreviousGunIndex(currentIndex));
    }

    public Player GetPlayer() { return player; }
}
