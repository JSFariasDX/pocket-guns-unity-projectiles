using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigWallet : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.CoinDropRateModifier += primaryReferenceAmount;
        base.onPlayerCollect(player);
    }
}
