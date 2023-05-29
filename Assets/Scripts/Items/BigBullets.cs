using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBullets : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.BulletSizeBonus += primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
