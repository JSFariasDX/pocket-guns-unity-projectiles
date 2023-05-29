using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflector : Special
{
    public override void OnActivate()
    {
        base.OnActivate();
        player.ProjectileInvulnerability = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.ProjectileInvulnerability = false;
    }
}
