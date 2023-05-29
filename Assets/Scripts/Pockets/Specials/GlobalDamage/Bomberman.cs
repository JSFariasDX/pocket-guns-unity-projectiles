using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomberman : Special
{
    [SerializeField]
    float globalDamageAmount = 20;
    public override void OnActivate()
    {
        base.OnActivate();
        GlobalDamage(globalDamageAmount);
    }
}
