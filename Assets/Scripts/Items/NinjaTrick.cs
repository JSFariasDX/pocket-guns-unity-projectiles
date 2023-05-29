using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaTrick : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.isPowerUpDash = true;
        base.onPlayerCollect(player);
    }
}
