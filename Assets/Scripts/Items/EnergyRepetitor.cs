using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyRepetitor : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.FirerateBonus -= .3f;
        base.onPlayerCollect(player);
    }
}
