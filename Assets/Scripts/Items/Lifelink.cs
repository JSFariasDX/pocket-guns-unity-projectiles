using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifelink : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.healOnKill = true;

        base.onPlayerCollect(player);
    }
}
