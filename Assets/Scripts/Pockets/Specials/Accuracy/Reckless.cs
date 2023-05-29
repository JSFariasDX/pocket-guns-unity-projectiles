using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reckless : Special
{

    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> bulletDistancePercent = new();
    [SerializeField] List<int> recoilPercent = new();
    [SerializeField] List<int> charMaxHealthPercent = new();

    float bulletDamage;
    float pushback;
    float bulletDistance;
    float recoil;
    float charMaxHealth;

    float secondaryBulletDamage;
    float secondaryPushback;
    float secondaryBulletDistance;
    float secondaryRecoil;
    float secondaryCharMaxHealth;

    private void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        bulletDistance = GetPercentValue(bulletDistancePercent[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercent[GetCurrentPet().level - 1]);
        charMaxHealth = GetPercentValue(charMaxHealthPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryPushback = pushback * .2f;
        secondaryBulletDistance = bulletDistance * .2f;
        secondaryRecoil = recoil * .2f;
        secondaryCharMaxHealth = charMaxHealth * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.DamageBonus += bulletDamage;
        player.PushBackPrevention += pushback;
        player.BulletDistanceBonus += bulletDistance;
        player.RecoilStabilization += recoil;
        player.MaxHPBonus += charMaxHealth;

        player.SetLaserSightOn(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.DamageBonus -= bulletDamage;
        player.PushBackPrevention -= pushback;
        player.BulletDistanceBonus -= bulletDistance;
        player.RecoilStabilization -= recoil;
        player.MaxHPBonus -= charMaxHealth;

        player.SetLaserSightOn(false);
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += secondaryBulletDamage;
        player.PushBackPrevention -= secondaryPushback;
        player.BulletDistanceBonus += secondaryBulletDistance;
        player.RecoilStabilization -= secondaryRecoil;
        player.MaxHPBonus += secondaryCharMaxHealth;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.PushBackPrevention += secondaryPushback;
        player.BulletDistanceBonus -= secondaryBulletDistance;
        player.RecoilStabilization += secondaryRecoil;
        player.MaxHPBonus -= secondaryCharMaxHealth;
    }
}
