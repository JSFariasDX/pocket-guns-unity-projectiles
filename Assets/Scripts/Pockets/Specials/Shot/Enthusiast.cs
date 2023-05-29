using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enthusiast : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> coinDropRatePercent = new();
    [SerializeField] List<int> bulletDamagePercent = new();

    float coinDrop;
    float bulletDamage;
    float secondaryCoinDrop;
    float secondaryBulletDamage;

    private void Start()
    {
        coinDrop = GetPercentValue(coinDropRatePercent[GetCurrentPet().level - 1]);
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);

        secondaryCoinDrop = coinDrop * .2f;
        secondaryBulletDamage = bulletDamage * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDrop;
        player.DamageBonus += bulletDamage;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(true);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.CoinDropRateModifier -= coinDrop;
        player.DamageBonus -= bulletDamage;

        List<Gun> guns = player.GetAllGuns();

        foreach (var item in guns)
        {
            item.SetTrail(false);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.CoinDropRateModifier += secondaryCoinDrop;
        player.DamageBonus += secondaryBulletDamage;

    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.CoinDropRateModifier -= secondaryCoinDrop;
        player.DamageBonus -= secondaryBulletDamage;
    }
}
