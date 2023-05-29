using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePlus : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> maxHPPercent = new();
    float maxHP;
    float secondaryMaxHP;

    private void Start()
    {
        maxHP = GetPercentValue(maxHPPercent[GetCurrentPet().level - 1]);
        secondaryMaxHP = maxHP * 0.2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.MaxHPBonus += maxHP;

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
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
