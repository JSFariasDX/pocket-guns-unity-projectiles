using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteransBandanna : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.DamageBonus += primaryReferenceAmount;
        player.GunRechargeBonus -= primaryReferenceAmount;
        player.BulletDistanceBonus += primaryReferenceAmount;

        player.RecoilStabilization -= 0.6f;
        base.onPlayerCollect(player);
    }
}
