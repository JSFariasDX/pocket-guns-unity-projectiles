using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HigherLifeTotal : Special
{
    [Header("Special custom parameters")]
    [SerializeField] int maxHPPercent = 20;
    float maxHP;
    float secondaryMaxHP;

    private void Start()
    {
        maxHP = GetPercentValue(maxHPPercent);
        secondaryMaxHP = maxHP * 0.2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.MaxHPBonus += maxHP;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.MaxHPBonus -= maxHP;
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.MaxHPBonus += secondaryMaxHP;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.MaxHPBonus -= secondaryMaxHP;
    }
}
