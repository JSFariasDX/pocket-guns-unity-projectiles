using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stability : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> bulletDistancePercent = new();
    [SerializeField] List<int> recoilPercent = new();

    float bulletDamage;
    float pushback;
    float bulletDistance;
    float recoil;

    float secondaryBulletDamage;
    float secondaryPushback;
    float secondaryBulletDistance;
    float secondaryRecoil;

    private void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        bulletDistance = GetPercentValue(bulletDistancePercent[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryPushback = pushback * .2f;
        secondaryBulletDistance = bulletDistance * .2f;
        secondaryRecoil = recoil * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.DamageBonus += bulletDamage;
        player.PushBackPrevention += pushback;
        player.BulletDistanceBonus += bulletDistance;
        player.RecoilStabilization += recoil;

        player.SetLaserSightOn(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.DamageBonus -= bulletDamage;
        player.PushBackPrevention -= pushback;
        player.BulletDistanceBonus -= bulletDistance;
        player.RecoilStabilization -= recoil;

        player.SetLaserSightOn(false);
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += secondaryBulletDamage;
        player.PushBackPrevention -= secondaryPushback;
        player.BulletDistanceBonus += secondaryBulletDistance;
        player.RecoilStabilization -= secondaryRecoil;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.PushBackPrevention += secondaryPushback;
        player.BulletDistanceBonus -= secondaryBulletDistance;
        player.RecoilStabilization += secondaryRecoil;
    }
}
