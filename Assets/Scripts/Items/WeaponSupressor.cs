using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSupressor : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.RecoilStabilization -= primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
