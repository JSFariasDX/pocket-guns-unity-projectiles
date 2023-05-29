using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mercenary : Special
{
    [Header("Special custom parameters")]
    [SerializeField] int coinDropPercent = 20;
    [SerializeField] int pushbackPercent = -5;
    [SerializeField] int bulletDistancePercent = 5;
    [SerializeField] int recoilPercent = -10;
    [SerializeField] int charMaxHealthPercent = -10;

    float coinDrop;
    float pushback;
    float bulletDistance;
    float recoil;
    float charMaxHealth;

    float secondaryCoinDrop;
    float secondaryPushback;
    float secondaryBulletDistance;
    float secondaryRecoil;
    float secondaryCharMaxHealth;

    private void Start()
    {
        coinDrop = GetPercentValue(coinDropPercent);
        pushback = GetPercentValue(pushbackPercent);
        bulletDistance = GetPercentValue(bulletDistance);
        recoil = GetPercentValue(recoilPercent);
        charMaxHealth = GetPercentValue(charMaxHealthPercent);

        secondaryCoinDrop = coinDrop * .2f;
        secondaryPushback = pushback * .2f;
        secondaryBulletDistance = bulletDistance * .2f;
        secondaryRecoil = recoil * .2f;
        secondaryCharMaxHealth = charMaxHealth * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.CoinDropRateModifier += coinDrop;
        player.PushBackPrevention -= pushback;
        player.BulletDistanceBonus += bulletDistance;
        player.RecoilStabilization -= recoil;
        player.MaxHPBonus += charMaxHealth;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.CoinDropRateModifier -= coinDrop;
        player.PushBackPrevention += pushback;
        player.BulletDistanceBonus -= bulletDistance;
        player.RecoilStabilization += recoil;
        player.MaxHPBonus -= charMaxHealth;
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.CoinDropRateModifier += secondaryCoinDrop;
        player.PushBackPrevention -= secondaryPushback;
        player.BulletDistanceBonus += secondaryBulletDistance;
        player.RecoilStabilization -= secondaryRecoil;
        player.MaxHPBonus += secondaryCharMaxHealth;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.CoinDropRateModifier -= secondaryCoinDrop;
        player.PushBackPrevention += secondaryPushback;
        player.BulletDistanceBonus -= secondaryBulletDistance;
        player.RecoilStabilization += secondaryRecoil;
        player.MaxHPBonus -= secondaryCharMaxHealth;
    }
}
