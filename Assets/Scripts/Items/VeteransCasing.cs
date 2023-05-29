using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteransCasing : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.BulletDistanceBonus += primaryReferenceAmount;

        base.onPlayerCollect(player);
    }
}
