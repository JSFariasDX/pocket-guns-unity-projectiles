using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Degravitizer : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        foreach (Player p in GameplayManager.Instance.GetPlayers(false))
        {
            p.hasDegravitizer = true;
            //p.GetComponentInChildren<JikiObject>().enabled = false;
        }

        base.onPlayerCollect(player);
    }
}
