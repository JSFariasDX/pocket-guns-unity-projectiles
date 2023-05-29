using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyBullets : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.ProjectilePierce = true;
        base.onPlayerCollect(player);
    }
}
