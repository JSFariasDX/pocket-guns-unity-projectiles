using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwiftfootBoots : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.MoveSpeedBonus += base.primaryReferenceAmount;
        player.MaxHPBonus += primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
