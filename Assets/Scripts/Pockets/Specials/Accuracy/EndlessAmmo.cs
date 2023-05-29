using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessAmmo : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> damagePerBulletPercentage = new();
    [SerializeField] List<int> ammoBonusPercentage = new();
    [SerializeField] List<int> rechargeTimePercentage = new();
    [SerializeField] List<int> fireRatePercentage = new();
    [SerializeField] List<int> pushbackPercentage = new();
    [SerializeField] List<int> recoilPercentage = new();
    [SerializeField] List<bool> projectilePierce = new();

    float damagePerBullet;
    float ammoBonus;
    float gunRechargeTime;
    float fireRate;
    float pushback;
    float recoil;

    bool powerUpApplied = false;

    private void Start()
    {
        damagePerBullet = GetPercentValue(damagePerBulletPercentage[GetCurrentPet().level - 1]);
        ammoBonus = GetPercentValue(ammoBonusPercentage[GetCurrentPet().level - 1]);
        gunRechargeTime = GetPercentValue(rechargeTimePercentage[GetCurrentPet().level - 1]);
        fireRate = GetPercentValue(fireRatePercentage[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercentage[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercentage[GetCurrentPet().level - 1]);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        base.player.DamageBonus += damagePerBullet;
        //base.player.AmmoBonus += ammoBonus;
        base.player.GunRechargeBonus += gunRechargeTime;
        base.player.FirerateBonus += fireRate;
        base.player.PushBackPrevention += pushback;
        base.player.RecoilStabilization += recoil;

        if (player.ProjectilePierce == false)
            player.ProjectilePierce = projectilePierce[GetCurrentPet().level - 1];
        else
            powerUpApplied = true;

        player.GetCurrentGun().InstantReload();

        player.SetLaserSightOn(true);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        base.player.DamageBonus -= damagePerBullet;
        //base.player.AmmoBonus -= ammoBonus;
        base.player.GunRechargeBonus -= gunRechargeTime;
        base.player.FirerateBonus -= fireRate;
        base.player.PushBackPrevention -= pushback;
        base.player.RecoilStabilization -= recoil;

        if(!powerUpApplied)
            player.ProjectilePierce = false;

        player.SetLaserSightOn(false);
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += ((float)(damagePerBullet * 0.2));
        //base.player.AmmoBonus += ((float)(ammoBonus * 0.2));
        base.player.GunRechargeBonus -= ((float)(gunRechargeTime * 0.2));
        base.player.FirerateBonus += ((float)(fireRate * 0.2));
        base.player.PushBackPrevention -= ((float)(pushback * 0.2));
        base.player.RecoilStabilization -= ((float)(recoil * 0.2));
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= ((float)(damagePerBullet * 0.2));
        //base.player.AmmoBonus -= ((float)(ammoBonus * 0.2));
        base.player.GunRechargeBonus += ((float)(gunRechargeTime * 0.2));
        base.player.FirerateBonus -= ((float)(fireRate * 0.2));
        base.player.PushBackPrevention += ((float)(pushback * 0.2));
        base.player.RecoilStabilization += ((float)(recoil * 0.2));
    }
}
