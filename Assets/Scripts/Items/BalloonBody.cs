using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBody : Collectible
{
    public override void onPlayerCollect(Player player)
    {
        player.ProjectilePierce = true;
        player.isPocketFlight = true;
        player.PushBackPrevention += primaryReferenceAmount;
        player.MoveSpeedBonus += .5f;
        base.onPlayerCollect(player);
    }
}
