using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> ammoPercentage = new();
    [SerializeField] List<int> rechargePercentage = new();
    [SerializeField] List<int> fireRatePercentage = new();
    [SerializeField] List<int> bulletDistancePercentage = new();
    [SerializeField] List<int> recoilPercentage = new();
    [SerializeField] List<int> charSpeedPercente = new();
    [SerializeField] List<bool> projectileFlight = new();
    [SerializeField] List<bool> projectilePierce = new();

    float ammo;
    float recharge;
    float fireRate;
    float bulletDistance;
    float recoil;
    float charSpeed;

    float secondaryAmmo;
    float secondaryRecharge;
    float secondaryFireRate;
    float secondaryBulletDistance;
    float secondaryRecoil;
    float secondaryCharSpeed;

    bool powerUpApplied = false;

    private void Start()
    {
        ammo = GetPercentValue(ammoPercentage[GetCurrentPet().level - 1]);
        recharge = GetPercentValue(rechargePercentage[GetCurrentPet().level - 1]);
        fireRate = GetPercentValue(fireRatePercentage[GetCurrentPet().level - 1]);
        bulletDistance = GetPercentValue(bulletDistancePercentage[GetCurrentPet().level - 1]);
        recoil = GetPercentValue(recoilPercentage[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercente[GetCurrentPet().level - 1]);

        secondaryAmmo = ammo * .2f;
        secondaryRecharge = recharge * .2f;
        secondaryFireRate = fireRate * .2f;
        secondaryBulletDistance = bulletDistance * .2f;
        secondaryRecoil = recoil * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        //player.AmmoBonus += ammo;
        player.GunRechargeBonus += recharge;
        player.FirerateBonus += fireRate;
        player.BulletDistanceBonus += bulletDistance;
        player.RecoilStabilization += recoil;
        player.MoveSpeedBonus += charSpeed;

        // dash -5%
        // dashSpeed -5%
        // dashTime -5%
        // dashCooldown +5%

        if (player.ProjectileFlight == false || player.ProjectilePierce == false)
        {
            player.ProjectileFlight = projectileFlight[GetCurrentPet().level - 1];
            player.ProjectilePierce = projectilePierce[GetCurrentPet().level - 1];
        }
        else
            powerUpApplied = true;

        player.SetLaserSightOn(true);

        Invoke("ReloadAllGuns", .1f);
    }

    void ReloadAllGuns()
    {
        List<Gun> guns = player.GetAllGuns();

        for (int i = 0; i < guns.Count; i++)
        {
            guns[i].InstantReload();
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();

        //player.AmmoBonus -= ammo;
        player.GunRechargeBonus -= recharge;
        player.FirerateBonus -= fireRate;
        player.BulletDistanceBonus -= bulletDistance;
        player.RecoilStabilization -= recoil;
        player.MoveSpeedBonus -= charSpeed;

        if (!powerUpApplied)
        {
            player.ProjectileFlight = false;
            player.ProjectilePierce = false;
        }

        player.SetLaserSightOn(false);
        player.GetCurrentGun().DeductBullets();

        ReloadAllGuns();
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        //player.AmmoBonus += secondaryAmmo;
        player.GunRechargeBonus -= secondaryRecharge;
        player.FirerateBonus += secondaryFireRate;
        player.BulletDistanceBonus += secondaryBulletDistance;
        player.RecoilStabilization -= secondaryRecoil;
        player.MoveSpeedBonus += secondaryCharSpeed;
        // dash -1%
        // dashSpeed -1%
        // dashTime -1%
        // dashCooldown +1%
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        //player.AmmoBonus -= secondaryAmmo;
        player.GunRechargeBonus += secondaryRecharge;
        player.FirerateBonus -= secondaryFireRate;
        player.BulletDistanceBonus -= secondaryBulletDistance;
        player.RecoilStabilization += secondaryRecoil;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
