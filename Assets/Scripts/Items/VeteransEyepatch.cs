using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteransEyepatch : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.WeaponDropRateModifier += primaryReferenceAmount;
        player.DamageBonus += primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
