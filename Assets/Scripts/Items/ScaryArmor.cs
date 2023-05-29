using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryArmor : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.AddShield((int)base.primaryReferenceAmount);
        base.onPlayerCollect(player);
    }
}
