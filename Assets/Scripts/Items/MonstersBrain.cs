using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersBrain : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.BulletDistanceBonus += primaryReferenceAmount;
        player.RecoilStabilization += 1;
        player.BulletSizeBonus += primaryReferenceAmount;

        player.WeaponDropRateModifier -= 1;
        player.DashCooldownModifier -= primaryReferenceAmount;

        base.onPlayerCollect(player);
    }
}
