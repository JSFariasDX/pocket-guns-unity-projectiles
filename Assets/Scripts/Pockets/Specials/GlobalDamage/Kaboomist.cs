using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaboomist : Special
{
    [SerializeField]
    List<float> globalDamageAmount = new();

    [SerializeField] int maxHpPercentage = -60;

    float maxHp;
    float secondaryMaxHP;

    private void Start()
    {
        maxHp = GetPercentValue(maxHpPercentage);
        secondaryMaxHP = maxHp * 0.2f;
    }
    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        GlobalDamage(globalDamageAmount[GetCurrentPet().level - 1]);
        player.MaxHPBonus += maxHp;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.MaxHPBonus -= maxHp;
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
