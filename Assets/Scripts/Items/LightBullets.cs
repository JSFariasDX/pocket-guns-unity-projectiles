using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBullets : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.PushBackPrevention -= base.primaryReferenceAmount;
        player.RecoilStabilization += base.primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
