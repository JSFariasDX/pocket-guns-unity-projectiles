using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForTheFun : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> coinDropRatePercent = new();
    [SerializeField] List<int> charSpeedPercent = new();

    float coinDropRate;
    float charSpeed;

    float secondaryCoinDropRate;
    float secondaryCharSpeed;

    SpecialParticles particle;

    private void Start()
    {
        coinDropRate = GetPercentValue(coinDropRatePercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryCoinDropRate = coinDropRate * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDropRate;
        player.MoveSpeedBonus += charSpeed;

        if (specialParticle)
        {
            particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.CoinDropRateModifier -= coinDropRate;
        player.MoveSpeedBonus -= charSpeed;

        if (particle != null)
        {
            Destroy(particle.gameObject);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.CoinDropRateModifier += secondaryCoinDropRate;
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.CoinDropRateModifier -= secondaryCoinDropRate;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
