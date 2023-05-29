using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialGloves : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.RecoilStabilization += primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
