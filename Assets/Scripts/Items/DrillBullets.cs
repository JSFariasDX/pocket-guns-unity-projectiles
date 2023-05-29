using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillBullets : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.ProjectileFlight = true;
        base.onPlayerCollect(player);
    }
}
