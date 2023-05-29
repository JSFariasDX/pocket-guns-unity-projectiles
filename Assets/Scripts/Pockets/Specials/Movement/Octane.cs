using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octane : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> charMaxHPPercent = new();
    [SerializeField] List<int> charSpeedPercent = new();
    [SerializeField] List<int> dashCooldownPercent = new();

    float charMaxHP;
    float charSpeed;
    float dashCooldown;

    float secondaryCharMaxHP;
    float secondaryCharSpeed;

    SpecialParticles particle;

    private void Start()
    {
        charMaxHP = GetPercentValue(charMaxHPPercent[GetCurrentPet().level - 1]);
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);
        dashCooldown = GetPercentValue(dashCooldownPercent[GetCurrentPet().level - 1]);

        secondaryCharMaxHP = charMaxHP * .2f;
        secondaryCharSpeed = charSpeed * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.MaxHPBonus += charMaxHP;
        player.MoveSpeedBonus += charSpeed;
        player.DashCooldownModifier += dashCooldown;

        if (specialParticle)
        {
            particle = Instantiate(specialParticle, transform.position, Quaternion.identity);
            particle.Setup(player.transform, useType == SpecialUseType.TimeBased ? totalTime[GetCurrentPet().level - 1] : 1);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        player.MaxHPBonus -= charMaxHP;
        player.MoveSpeedBonus -= charSpeed;
        player.DashCooldownModifier -= dashCooldown;

        if (particle != null)
        {
            Destroy(particle.gameObject);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.MaxHPBonus += secondaryCharMaxHP;
        player.MoveSpeedBonus += secondaryCharSpeed;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.MaxHPBonus -= secondaryCharMaxHP;
        player.MoveSpeedBonus -= secondaryCharSpeed;
    }
}
