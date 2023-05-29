using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafBlower : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        foreach (Player p in GameplayManager.Instance.GetPlayers(false))
        {
            p.hasLeafBlower = true;
            p.GetComponentInChildren<WindObject>().enabled = false;
        }
        
        base.onPlayerCollect(player);
    }
}
