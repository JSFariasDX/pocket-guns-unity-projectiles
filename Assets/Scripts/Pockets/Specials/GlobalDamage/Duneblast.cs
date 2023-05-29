using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duneblast : Special
{
    [SerializeField]
    List<float> globalDamageAmount = new();
    [SerializeField] List<int> shields = new();
    public override void OnActivate()
    {
        base.OnActivate();
        player.AddShield(shields[GetCurrentPet().level - 1]);
        GlobalDamage(globalDamageAmount[GetCurrentPet().level - 1]);
    }
}
