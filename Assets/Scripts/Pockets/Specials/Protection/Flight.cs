using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flight : Special
{
    public override void OnActivate()
    {
        base.OnActivate();
        player.isPocketFlight = true;
        player.Flight = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
