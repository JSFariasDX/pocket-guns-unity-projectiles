using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectiles : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.homingProjectiles = true;

        base.onPlayerCollect(player);
    }
}
