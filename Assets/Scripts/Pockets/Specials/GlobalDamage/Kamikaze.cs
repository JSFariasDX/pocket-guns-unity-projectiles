using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamikaze : Special
{
    [SerializeField]
    float globalDamageAmount = 200;

    public override void OnActivate()
    {
        base.OnActivate();
        GlobalDamage(globalDamageAmount);
    }
}
