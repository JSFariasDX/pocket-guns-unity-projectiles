using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteransGear : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.DamageBonus += primaryReferenceAmount;
        player.AmmoBonus += primaryReferenceAmount;
        player.GunRechargeBonus -= primaryReferenceAmount;
        player.BulletDistanceBonus += primaryReferenceAmount;
        player.FirerateBonus += primaryReferenceAmount;
        player.PushBackPrevention -= primaryReferenceAmount;

        player.MoveSpeedBonus -= 0.2f;

        List<Gun> guns = player.GetAllGuns();

        for (int i = 0; i < guns.Count; i++)
        {
            guns[i].InstantReload();
        }

        base.onPlayerCollect(player);
    }
}
