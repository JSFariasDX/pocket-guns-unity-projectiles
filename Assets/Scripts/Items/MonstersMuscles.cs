using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstersMuscles : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        if (player.GetHealth().GetCurrentPercentage() <= .05f) return;

        player.DashCooldownModifier -= primaryReferenceAmount;

        player.GetHealth().Decrease(50);

        base.onPlayerCollect(player);
    }
}
