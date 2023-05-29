using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearingCare : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> charHealingPercent = new();
    [SerializeField] List<int> primaryPocketHealingPercent = new();
    [SerializeField] List<int> bulletDamagePercent = new();

    float charHealing;
    float primaryPocketHealing;
    float bulletDamage;
    float secondaryBulletDamage;

    private void Start()
    {
        charHealing = GetPercentValue(charHealingPercent[GetCurrentPet().level - 1]);
        primaryPocketHealing = GetPercentValue(primaryPocketHealingPercent[GetCurrentPet().level - 1]);
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        secondaryBulletDamage = bulletDamage * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.GetHealth().Heal(charHealing);
        player.GetCurrentPocket().GetHealth().Heal(primaryPocketHealing);
        player.DamageBonus += bulletDamage;

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.DamageBonus -= bulletDamage;
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += secondaryBulletDamage;

    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
    }
}
