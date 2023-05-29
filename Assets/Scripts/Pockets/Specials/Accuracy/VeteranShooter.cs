using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VeteranShooter : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> ammoPercent = new();
    [SerializeField] List<int> fireRatePercent = new();
    [SerializeField] List<int> pushbackPercent = new();
    [SerializeField] List<int> recoilPercent = new();
    [SerializeField] List<int> charMaxHPPercent = new();
    [SerializeField] List<bool> projectileFlight = new();
    [SerializeField] List<int> shields = new();

    float ammo;
    float fireRate;
    float pushback;
    float recoil;
    float maxHP;

    float secondaryAmmo;
    float secondaryFireRate;
    float secondaryPushback;
    float secondaryRecoil;
    float secondaryMaxHP;

    bool powerUpApplied = false;

    private void Start()
    {
        ammo = GetPercentValue(ammoPercent[GetCurrentPet().level - 1]);
        fireRate = GetPercentValue(fireRatePercent[GetCurrentPet().level - 1]);
        pushback = GetPercentValue(pushbackPercent[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercent[GetCurrentPet().level - 1]);
        maxHP = GetPercentValue(charMaxHPPercent[GetCurrentPet().level - 1]);

        secondaryAmmo = ammo * .2f;
        secondaryFireRate = fireRate * .2f;
        secondaryPushback = pushback * .2f;
        secondaryRecoil = recoil * .2f;
        secondaryMaxHP = maxHP * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        //player.AmmoBonus += ammo;
        player.FirerateBonus += fireRate;
        player.PushBackPrevention += pushback;
        player.RecoilStabilization += recoil;
        player.MaxHPBonus += maxHP;

        if (player.ProjectileFlight == false)
            player.ProjectileFlight = projectileFlight[GetCurrentPet().level - 1];
        else
            powerUpApplied = true;

        player.AddShield(shields[GetCurrentPet().level - 1]);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        //player.AmmoBonus -= ammo;
        player.FirerateBonus -= fireRate;
        player.PushBackPrevention -= pushback;
        player.RecoilStabilization -= recoil;
        player.MaxHPBonus -= maxHP;

        if(!powerUpApplied)
            player.ProjectileFlight = false;
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        //player.AmmoBonus += secondaryAmmo;
        player.FirerateBonus += secondaryFireRate;
        player.PushBackPrevention -= secondaryPushback;
        player.RecoilStabilization -= secondaryRecoil;
        player.MaxHPBonus += secondaryMaxHP;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        //player.AmmoBonus -= secondaryAmmo;
        player.FirerateBonus -= secondaryFireRate;
        player.PushBackPrevention += secondaryPushback;
        player.RecoilStabilization += secondaryRecoil;
        player.MaxHPBonus -= secondaryMaxHP;
    }
}
