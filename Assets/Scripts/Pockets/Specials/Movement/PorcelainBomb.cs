using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorcelainBomb : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> charMaxHPPercent = new();
    [SerializeField] List<int> charSpeedPercent = new();

    float bulletDamage;
    float charMaxHP;
    float charSpeed;

    float secondaryBulletDamage;
    float secondaryCharMaxHP;
    float secondaryCharSpeed;

    SpecialParticles particle;

    private void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        charMaxHP = GetPercentValue(charMaxHPPercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryCharMaxHP = charMaxHP * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.DamageBonus += bulletDamage;
        player.MaxHPBonus += charMaxHP;
        player.MoveSpeedBonus += charSpeed;

        if (specialParticle)
        {
            particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.DamageBonus -= bulletDamage;
        player.MaxHPBonus -= charMaxHP;
        player.MoveSpeedBonus -= charSpeed;

        if (particle != null)
        {
            Destroy(particle.gameObject);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.DamageBonus += secondaryBulletDamage;
        player.MaxHPBonus += secondaryCharMaxHP;
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.MaxHPBonus -= secondaryCharMaxHP;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
