using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbox : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.AmmoBonus += base.primaryReferenceAmount;

        List<Gun> guns = player.GetAllGuns();

        for (int i = 0; i < guns.Count; i++)
        {
            guns[i].InstantReload();
        }

        base.onPlayerCollect(player);
    }
}
