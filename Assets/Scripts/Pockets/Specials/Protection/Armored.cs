using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armored : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> shields = new();

    private void Start()
    {

    }

    public override void OnActivate()
    {
        base.OnActivate();
        player.AddShield(shields[GetCurrentPet().level - 1]);
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
    }
}
