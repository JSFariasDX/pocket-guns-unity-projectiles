using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcceleratorBuckle : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.MoveSpeedBonus += base.primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
