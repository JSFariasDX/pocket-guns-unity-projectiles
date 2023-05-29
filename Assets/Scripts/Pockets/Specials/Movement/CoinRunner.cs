using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRunner : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> coinDropRatePercent = new();
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> charSpeedPercent = new();

    float coinDropRate;
    float bulletDamage;
    float charSpeed;

    float secondaryCoinDropRate;
    float secondaryBulletDamage;
    float secondaryCharSpeed;

    SpecialParticles particle;

    private void Start()
    {
        coinDropRate = GetPercentValue(coinDropRatePercent[GetCurrentPet().level - 1]);
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryCoinDropRate = coinDropRate * .2f;
        secondaryBulletDamage = bulletDamage * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDropRate;
        player.DamageBonus += bulletDamage;
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
        player.DamageBonus -= bulletDamage;
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
        player.DamageBonus += secondaryBulletDamage;
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.CoinDropRateModifier -= secondaryCoinDropRate;
        player.DamageBonus -= secondaryBulletDamage;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
