using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marathon : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> bulletDamagePercent = new();
    [SerializeField] List<int> charSpeedPercent = new();

    float bulletDamage;
    float charSpeed;

    float secondaryBulletDamage;
    float secondaryCharSpeed;

    SpecialParticles particle;

    private void Start()
    {
        bulletDamage = GetPercentValue(bulletDamagePercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);

        secondaryBulletDamage = bulletDamage * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.DamageBonus += bulletDamage;
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
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.DamageBonus -= secondaryBulletDamage;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
