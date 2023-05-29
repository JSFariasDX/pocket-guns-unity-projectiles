using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsceticShot : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> coinDropRatePercentage = new();
    [SerializeField] List<int> mementoDropRatePercentage = new();
    [SerializeField] List<int> weaponDropRatePercentage = new();
    [SerializeField] List<int> powerUpDropRatePercentage = new();

    [SerializeField] List<int> rechargeTimePercentage = new();
    [SerializeField] List<int> fireRatePercentage = new();
    [SerializeField] List<int> pushbackPercentage = new();
    [SerializeField] List<int> bulletDistancePercentage = new();
    [SerializeField] List<int> recoilPercentage = new();
    [SerializeField] List<bool> projectileFlight = new();

    float coinDrop;
    float mementoDrop;
    float weaponDrop;
    float powerUpDrop;
    float gunRechargeTime;
    float fireRate;
    float pushback;
    float bulletDistance;
    float recoil;

    float secondaryCoinDrop;
    float secondaryMementoDrop;
    float secondaryWeaponDrop;
    float secondaryPowerUpDrop;
    float secondaryRechargeTime;
    float secondaryFireRate;
    float secondaryPushback;
    float secondaryShotDistance;
    float secondaryRecoil;

    private void Start()
    {
        coinDrop = GetPercentValue(coinDropRatePercentage[GetCurrentPet().level - 1]);
        mementoDrop = GetPercentValue(mementoDropRatePercentage[GetCurrentPet().level - 1]);
        weaponDrop = GetPercentValue(weaponDropRatePercentage[GetCurrentPet().level - 1]);
        powerUpDrop = GetPercentValue(powerUpDropRatePercentage[GetCurrentPet().level - 1]);
        gunRechargeTime = GetPercentValue(rechargeTimePercentage[GetCurrentPet().level - 1]);
        fireRate = GetPercentValue(fireRatePercentage[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercentage[GetCurrentPet().level - 1]);
        bulletDistance = GetPercentValue(bulletDistancePercentage[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercentage[GetCurrentPet().level - 1]);

        secondaryCoinDrop = coinDrop * .2f;
        secondaryMementoDrop = mementoDrop * .2f;
        secondaryWeaponDrop = weaponDrop * .2f;
        secondaryPowerUpDrop = powerUpDrop * .2f;
        secondaryRechargeTime = gunRechargeTime * .2f;
        secondaryFireRate = fireRate * .2f;
        secondaryPushback = pushback * .2f;
        secondaryShotDistance = bulletDistance * .2f;
        secondaryRecoil = recoil * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDrop;
        player.MementoDropRateModifier += mementoDrop;
        player.WeaponDropRateModifier += weaponDrop;
        player.PowerUpDropRateModifier += powerUpDrop;
        player.GunRechargeBonus += gunRechargeTime;
        player.FirerateBonus += fireRate;
        player.PushBackPrevention += pushback;
        player.BulletDistanceBonus += bulletDistance;
        player.RecoilStabilization += recoil;

        player.ProjectileFlight = projectileFlight[GetCurrentPet().level - 1];

        player.SetLaserSightOn(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();

        player.CoinDropRateModifier -= coinDrop;
        player.MementoDropRateModifier -= mementoDrop;
        player.WeaponDropRateModifier -= weaponDrop;
        player.PowerUpDropRateModifier -= powerUpDrop;
        player.GunRechargeBonus -= gunRechargeTime;
        player.FirerateBonus -= fireRate;
        player.PushBackPrevention += pushback;
        player.BulletDistanceBonus -= bulletDistance;
        player.RecoilStabilization -= recoil;

        player.ProjectileFlight = false;

        player.SetLaserSightOn(false);
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();

        player.CoinDropRateModifier += secondaryCoinDrop;
        player.MementoDropRateModifier += secondaryMementoDrop;
        player.WeaponDropRateModifier += secondaryWeaponDrop;
        player.PowerUpDropRateModifier += secondaryPowerUpDrop;
        player.GunRechargeBonus -= secondaryRechargeTime;
        player.FirerateBonus += secondaryFireRate;
        player.PushBackPrevention -= secondaryPushback;
        player.BulletDistanceBonus += secondaryShotDistance;
        player.RecoilStabilization -= secondaryRecoil;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();

        player.CoinDropRateModifier -= secondaryCoinDrop;
        player.MementoDropRateModifier -= secondaryMementoDrop;
        player.WeaponDropRateModifier -= secondaryWeaponDrop;
        player.PowerUpDropRateModifier -= secondaryPowerUpDrop;
        player.GunRechargeBonus += secondaryRechargeTime;
        player.FirerateBonus -= secondaryFireRate;
        player.PushBackPrevention += secondaryPushback;
        player.BulletDistanceBonus -= secondaryShotDistance;
        player.RecoilStabilization += secondaryRecoil;
    }
}
