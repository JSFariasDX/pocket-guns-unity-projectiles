using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restoration : Special
{
    [Header("Special custom parameters")]
    [SerializeField] int charHealingPercent = 100;
    [SerializeField] int secondaryDamagePerBulletPercent = -5;
    [SerializeField] int secondaryBulletAmountPercent = -5;
    [SerializeField] int secondaryRechargeTimePercent = 5;
    [SerializeField] int secondaryBulletForcePercent = -5;
    [SerializeField] int secondaryCharSpeedPercent = -5;

    float charHealing;
    float damagePerBullet;
    float bulletAmount;
    float rechargeTimeBonus;
    float bulletForce;
    float charSpeed;

    private void Start()
    {
        charHealing = GetPercentValue(charHealingPercent);
        damagePerBullet = GetPercentValue(secondaryDamagePerBulletPercent);
        bulletAmount = GetPercentValue(secondaryBulletAmountPercent);
        rechargeTimeBonus = GetPercentValue(secondaryRechargeTimePercent);
        bulletForce = GetPercentValue(secondaryBulletForcePercent);
        charSpeed = GetPercentValue(secondaryCharSpeedPercent);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.GetHealth().Heal(charHealing);

        if (specialParticle)
        {
            SpecialParticles particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += damagePerBullet;
        //player.AmmoBonus += bulletAmount;
        player.GunRechargeBonus -= rechargeTimeBonus;
        player.BulletForceBonus += bulletForce;
        player.MoveSpeedBonus += charSpeed;
        // dashDistance -2%
        // dashSpeed -2%
        // dashCooldown +5%
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= damagePerBullet;
        //player.AmmoBonus -= bulletAmount;
        player.GunRechargeBonus += rechargeTimeBonus;
        player.BulletForceBonus -= bulletForce;
        player.MoveSpeedBonus -= charSpeed;
    }
}
