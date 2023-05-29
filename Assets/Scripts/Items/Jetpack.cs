using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.isPocketFlight = true;
        base.onPlayerCollect(player);
    }
}
