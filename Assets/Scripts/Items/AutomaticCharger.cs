using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticCharger : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.GunRechargeBonus += base.primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
