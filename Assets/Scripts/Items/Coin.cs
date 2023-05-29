using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        GameplayManager.Instance.AddCoins((int)base.primaryReferenceAmount);
        
        base.onPlayerCollect(player);
    }
}
