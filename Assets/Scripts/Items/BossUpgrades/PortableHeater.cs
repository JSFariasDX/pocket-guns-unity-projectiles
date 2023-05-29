using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortableHeater : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        foreach (Player p in GameplayManager.Instance.GetPlayers(false))
        {
            p.ResetTemperature(true);
        }
        
        base.onPlayerCollect(player);
    }
}
