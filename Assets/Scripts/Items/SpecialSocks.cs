using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSocks : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.PushBackPrevention -= primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
