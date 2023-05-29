using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialStrike : Special
{
    [SerializeField]
    List<float> globalDamageAmount = new();
    public override void OnActivate()
    {
        base.OnActivate();
        GlobalDamage(globalDamageAmount[GetCurrentPet().level - 1]);
    }
}
