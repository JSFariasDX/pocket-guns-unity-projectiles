using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greedy : Special
{
    [SerializeField] int secondaryCoinDropPercent = 4;
    float secondaryCoinDrop;

    private void Start()
    {
        secondaryCoinDrop = GetPercentValue(secondaryCoinDropPercent);
    }
    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        DropCoins dropCoin = GetComponentInParent<DropCoins>();
        dropCoin.DropCoin(player.transform.position, true);

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.CoinDropRateModifier += secondaryCoinDrop;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.CoinDropRateModifier -= secondaryCoinDrop;
    }
}
