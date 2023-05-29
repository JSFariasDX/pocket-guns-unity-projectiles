using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterClaws : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.damageOnCollision = true;
        base.onPlayerCollect(player);
    }
}
