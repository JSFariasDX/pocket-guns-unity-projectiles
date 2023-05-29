using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnipersEverywhere : Special
{
    [SerializeField]
    List<float> globalDamageAmount = new();

    [SerializeField] int secondaryAmmoBonusPercentage = -2;
    [SerializeField] int secondaryBulletForcePercentage = 5;
    [SerializeField] int secondaryBulletDistancePercentage = 10;
    [SerializeField] int secondaryRecoilPercentage = 10;
    [SerializeField] int secondaryProjectileSizePercentage = -10;

    float secondaryAmmoBonus;
    float secondaryBulletForce;
    float secondaryBulletDistance;
    float secondaryRecoil;
    float secondaryProjectileSize;

    private void Start()
    {
        secondaryAmmoBonus = GetPercentValue(secondaryAmmoBonusPercentage);
        secondaryBulletForce = GetPercentValue(secondaryBulletForcePercentage);
        secondaryBulletDistance = GetPercentValue(secondaryBulletDistancePercentage);
        secondaryRecoil = GetPercentValue(secondaryRecoilPercentage);
        secondaryProjectileSize = GetPercentValue(secondaryProjectileSizePercentage);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        GlobalDamage(globalDamageAmount[GetCurrentPet().level - 1]);
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        //player.AmmoBonus += secondaryAmmoBonus;
        player.BulletForceBonus += secondaryBulletForce;
        player.BulletDistanceBonus += secondaryBulletDistance;
        player.RecoilStabilization -= secondaryRecoil;
        player.BulletSizeBonus += secondaryProjectileSize;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        //player.AmmoBonus -= secondaryAmmoBonus;
        player.BulletForceBonus -= secondaryBulletForce;
        player.BulletDistanceBonus -= secondaryBulletDistance;
        player.RecoilStabilization += secondaryRecoil;
        player.BulletSizeBonus -= secondaryProjectileSize;
    }
}
