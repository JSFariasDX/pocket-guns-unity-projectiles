using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrink : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.DashCooldownModifier -= primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
