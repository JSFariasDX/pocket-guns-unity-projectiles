using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruelBullets : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.DamageBonus += base.primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
