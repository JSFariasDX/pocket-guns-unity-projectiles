using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrecisionPaid : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> coinDropPercent = new();
    [SerializeField] List<int> mementoDropPercent = new();
    [SerializeField] List<int> weaponDropPercent = new();
    [SerializeField] List<int> powerUpDropPercent = new();
    [SerializeField] List<int> fireRatePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> recoilPercent = new();
    [SerializeField] List<int> bulletDistancePercent = new();
    [SerializeField] List<int> charSpeedPercent = new();

    float coinDrop;
    float mementoDrop;
    float weaponDrop;
    float powerUpDrop;
    float fireRate;
    float pushback;
    float recoil;
    float bulletDistance;
    float charSpeed;

    float secondaryCoinDrop;
    float secondaryMementoDrop;
    float secondaryWeaponDrop;
    float secondaryPowerUpDrop;
    float secondaryFireRate;
    float secondaryPushback;
    float secondaryRecoil;
    float secondaryBulletDistance;
    float secondaryCharSpeed;

    private void Start()
    {
        coinDrop = GetPercentValue(coinDropPercent[GetCurrentPet().level - 1]);
        mementoDrop = GetPercentValue(mementoDropPercent[GetCurrentPet().level - 1]);
        weaponDrop = GetPercentValue(weaponDropPercent[GetCurrentPet().level - 1]);
        powerUpDrop = GetPercentValue(powerUpDropPercent[GetCurrentPet().level - 1]);
        fireRate = GetPercentValue(fireRatePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercent[GetCurrentPet().level - 1]);
        bulletDistance = GetPercentValue(bulletDistancePercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryCoinDrop = coinDrop * .2f;
        secondaryMementoDrop = mementoDrop * .2f;
        secondaryWeaponDrop = weaponDrop * .2f;
        secondaryPowerUpDrop = powerUpDrop * .2f;
        secondaryFireRate = fireRate * .2f;
        secondaryPushback = pushback * .2f;
        secondaryRecoil = recoil * .2f;
        secondaryBulletDistance = bulletDistance * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDrop;
        player.MementoDropRateModifier += mementoDrop;
        player.WeaponDropRateModifier += weaponDrop;
        player.PowerUpDropRateModifier += powerUpDrop;
        player.FirerateBonus += fireRate;
        player.PushBackPrevention -= pushback;
        player.RecoilStabilization -= recoil;
        player.BulletDistanceBonus += bulletDistance;
        player.MoveSpeedBonus += charSpeed;

        player.SetLaserSightOn(true);

    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.CoinDropRateModifier -= coinDrop;
        player.MementoDropRateModifier -= mementoDrop;
        player.WeaponDropRateModifier -= weaponDrop;
        player.PowerUpDropRateModifier -= powerUpDrop;
        player.FirerateBonus -= fireRate;
        player.PushBackPrevention += pushback;
        player.RecoilStabilization += recoil;
        player.BulletDistanceBonus -= bulletDistance;
        player.MoveSpeedBonus -= charSpeed;

        player.SetLaserSightOn(false);

    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.CoinDropRateModifier += secondaryCoinDrop;
        player.MementoDropRateModifier += secondaryMementoDrop;
        player.WeaponDropRateModifier += secondaryWeaponDrop;
        player.PowerUpDropRateModifier += secondaryPowerUpDrop;
        player.FirerateBonus += secondaryFireRate;
        player.PushBackPrevention -= secondaryPushback;
        player.RecoilStabilization -= secondaryRecoil;
        player.BulletDistanceBonus += secondaryBulletDistance;
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.CoinDropRateModifier -= secondaryCoinDrop;
        player.MementoDropRateModifier -= secondaryMementoDrop;
        player.WeaponDropRateModifier -= secondaryWeaponDrop;
        player.PowerUpDropRateModifier -= secondaryPowerUpDrop;
        player.FirerateBonus -= secondaryFireRate;
        player.PushBackPrevention += secondaryPushback;
        player.RecoilStabilization += secondaryRecoil;
        player.BulletDistanceBonus -= secondaryBulletDistance;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
