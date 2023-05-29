using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupidArrow : Special
{
    [Header("Special custom parameters")]
    [SerializeField] int charHealingPercent = 10;
    [SerializeField] int primaryPocketHealingPercent = 10;
    [SerializeField] int secondaryPocketsHealingPercent = 10;
    [SerializeField] int secondaryAmmoBonusPercent = 5;
    [SerializeField] int secondaryBulletForcePercent = 5;
    [SerializeField] int secondaryFireRatePercent = 2;
    [SerializeField] int secondaryPushbackPercent = -2;
    [SerializeField] int secondaryBulletDistancePercent = 10;

    float charHealing;
    float primaryPocketHealing;
    float secondaryPocketHealing;

    float secondaryAmmoBonus;
    float secondaryBulletForce;
    float secondaryFireRate;
    float secondaryPushback;
    float secondaryBulletDistance;

    private void Start()
    {
        charHealing = GetPercentValue(charHealingPercent);
        primaryPocketHealing = GetPercentValue(primaryPocketHealingPercent);
        secondaryPocketHealing = GetPercentValue(secondaryPocketsHealingPercent);

        secondaryAmmoBonus = GetPercentValue(secondaryAmmoBonusPercent);
        secondaryBulletForce = GetPercentValue(secondaryBulletForcePercent);
        secondaryFireRate = GetPercentValue(secondaryFireRatePercent);
        secondaryPushback = GetPercentValue(secondaryPushbackPercent);
        secondaryBulletDistance = GetPercentValue(secondaryBulletDistancePercent);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.GetHealth().Heal(charHealing);
        player.GetCurrentPocket().GetHealth().Heal(primaryPocketHealing);
        List<Pocket> secondarys = player.GetSecondaryPockets();
        foreach (var pocket in secondarys)
        {
            pocket.GetHealth().Heal(secondaryPocketHealing);
        }
        player.ProjectileFlight = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.ProjectileFlight = false;
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        //player.AmmoBonus += secondaryAmmoBonus;
        player.BulletForceBonus += secondaryBulletForce;
        player.FirerateBonus += secondaryFireRate;
        player.PushBackPrevention -= secondaryPushback;
        player.BulletDistanceBonus += secondaryBulletDistance;

    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        //player.AmmoBonus -= secondaryAmmoBonus;
        player.BulletForceBonus -= secondaryBulletForce;
        player.FirerateBonus -= secondaryFireRate;
        player.PushBackPrevention += secondaryPushback;
        player.BulletDistanceBonus -= secondaryBulletDistance;
    }
}
