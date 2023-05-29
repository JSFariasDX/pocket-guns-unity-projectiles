using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersLeg : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.PowerUpDropRateModifier += primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
