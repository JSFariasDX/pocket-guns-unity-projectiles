using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadeye : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> coinDropPercent = new();
    [SerializeField] List<int> fireRatePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> shotDistancePercent = new();
    [SerializeField] List<int> recoilPercent = new();

    float coinDrop;
    float fireRate;
    float pushback;
    float shotDistance;
    float recoil;

    float secondaryCoinDrop;
    float secondaryFireRate;
    float secondaryPushback;
    float secondaryShotDistance;
    float secondaryRecoil;

    private void Start()
    {
        coinDrop = GetPercentValue(coinDropPercent[GetCurrentPet().level - 1]);
        fireRate = GetPercentValue(fireRatePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercent[GetCurrentPet().level - 1]);
        shotDistance = GetPercentValue(shotDistancePercent[GetCurrentPet().level - 1]);

        secondaryCoinDrop = coinDrop * .2f;
        secondaryFireRate = fireRate * .2f;
        secondaryPushback = pushback * .2f;
        secondaryRecoil = recoil * .2f;
        secondaryShotDistance = shotDistance * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDrop;
        player.FirerateBonus += fireRate;
        player.PushBackPrevention += pushback;
        player.BulletDistanceBonus += shotDistance;
        player.RecoilStabilization += recoil;

        player.SetLaserSightOn(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.CoinDropRateModifier -= coinDrop;
        player.FirerateBonus -= fireRate;
        player.PushBackPrevention -= pushback;
        player.BulletDistanceBonus -= shotDistance;
        player.RecoilStabilization -= recoil;

        player.SetLaserSightOn(false);
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.CoinDropRateModifier += secondaryCoinDrop;
        player.FirerateBonus += secondaryFireRate;
        player.PushBackPrevention -= secondaryPushback;
        player.BulletDistanceBonus += secondaryShotDistance;
        player.RecoilStabilization -= secondaryRecoil;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.CoinDropRateModifier -= secondaryCoinDrop;
        player.FirerateBonus -= secondaryFireRate;
        player.PushBackPrevention += secondaryPushback;
        player.BulletDistanceBonus -= secondaryShotDistance;
        player.RecoilStabilization += secondaryRecoil;
    }
}
