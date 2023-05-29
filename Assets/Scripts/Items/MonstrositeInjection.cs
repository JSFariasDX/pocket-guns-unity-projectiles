using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonstrositeInjection : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.MoveSpeedBonus += .2f;
        Pocket pocket = player.GetCurrentPocket();
        if (pocket != null)
        {
            Health pocketHealth = pocket.GetComponent<Health>();
            pocketHealth.SetInvulnerability(true);
        }
        player.DamageBonus += .2f;
        player.PushBackPrevention += 1f;
        player.RecoilStabilization = 1f;
        player.OneHitKill = true;
        base.onPlayerCollect(player);
    }
}
