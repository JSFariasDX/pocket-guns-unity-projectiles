using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortLived : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> coinDropPercent = new();
    [SerializeField] List<int> mementoDropPercent = new();
    [SerializeField] List<int> weaponDropPercent = new();
    [SerializeField] List<int> powerUpDropPercent = new();

    [SerializeField] List<int> bulletDamagePercentage = new();
    [SerializeField] List<int> healthPercentage = new();

    float coinDrop;
    float mementoDrop;
    float weaponDrop;
    float powerUpDrop;
    float damage;
    float health;

    float secondaryCoinDrop;
    float secondaryMementoDrop;
    float secondaryWeaponDrop;
    float secondaryPowerUpDrop;
    float secondaryDamage;
    float secondaryHealth;

    private void Start()
    {
        coinDrop = GetPercentValue(coinDropPercent[GetCurrentPet().level - 1]);
        mementoDrop = GetPercentValue(mementoDropPercent[GetCurrentPet().level - 1]);
        weaponDrop = GetPercentValue(weaponDropPercent[GetCurrentPet().level - 1]);
        powerUpDrop = GetPercentValue(powerUpDropPercent[GetCurrentPet().level - 1]);
        damage = GetPercentValue(bulletDamagePercentage[GetCurrentPet().level - 1]);
        health = GetPercentValue(healthPercentage[GetCurrentPet().level - 1]);

        secondaryCoinDrop = coinDrop * .2f;
        secondaryMementoDrop = mementoDrop * .2f;
        secondaryWeaponDrop = weaponDrop * .2f;
        secondaryPowerUpDrop = powerUpDrop * .2f;
        secondaryDamage = damage * .2f;
        secondaryHealth = health * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDrop;
        player.MementoDropRateModifier += mementoDrop;
        player.WeaponDropRateModifier += weaponDrop;
        player.PowerUpDropRateModifier += powerUpDrop;
        player.DamageBonus += damage;
        player.MaxHPBonus += health;

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
        player.MementoDropRateModifier -= mementoDrop;
        player.WeaponDropRateModifier -= weaponDrop;
        player.PowerUpDropRateModifier -= powerUpDrop;
        player.DamageBonus -= damage;
        player.MaxHPBonus -= health;

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
        player.MementoDropRateModifier += secondaryMementoDrop;
        player.WeaponDropRateModifier += secondaryWeaponDrop;
        player.PowerUpDropRateModifier += secondaryPowerUpDrop;
        player.DamageBonus += secondaryDamage;
        player.MaxHPBonus += secondaryHealth;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();

        player.CoinDropRateModifier -= secondaryCoinDrop;
        player.MementoDropRateModifier -= secondaryMementoDrop;
        player.WeaponDropRateModifier -= secondaryWeaponDrop;
        player.PowerUpDropRateModifier -= secondaryPowerUpDrop;
        player.DamageBonus -= secondaryDamage;
        player.MaxHPBonus -= secondaryHealth;
    }
}
