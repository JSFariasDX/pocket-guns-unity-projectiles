using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adrenaline : Special
{
    [Header("Special custom parameters")]
    [SerializeField] List<int> charSpeedPercent = new();
    [SerializeField] List<int> dashSpeedPercent = new();
    [SerializeField] List<int> dashDurationPercent = new();
    [SerializeField] List<int> dashCooldownPercent = new();

    float charSpeed;
    float dashSpeed;
    float dashDuration;
    float dashCooldown;

    float secondaryCharSpeed;
    float secondaryDashSpeed;
    float secondaryDashDuration;
    float secondaryDashCooldown;

    SpecialParticles particle;

    private void Start()
    {
        charSpeed = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);
        dashSpeed = GetPercentValue(dashSpeedPercent[GetCurrentPet().level - 1]);
        dashDuration = GetPercentValue(charSpeedPercent[GetCurrentPet().level - 1]);
        dashCooldown = GetPercentValue(dashCooldownPercent[GetCurrentPet().level - 1]);

        secondaryCharSpeed = charSpeed * .2f;
        secondaryDashSpeed = dashSpeed * .2f;
        secondaryDashDuration = dashDuration * .2f;
        secondaryDashCooldown = dashCooldown * .2f;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        Start();
        player.MoveSpeedBonus += charSpeed;
        player.DashSpeedModifier += dashSpeed;
        player.DashDurationModifier += dashDuration;
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
        player.MoveSpeedBonus -= charSpeed;
        player.DashSpeedModifier -= dashSpeed;
        player.DashDurationModifier -= dashDuration;
        player.DashCooldownModifier -= dashCooldown;

        if(particle != null)
        {
            Destroy(particle.gameObject);
        }
    }

    public override void ApplySecondaryEffect()
    {
        base.ApplySecondaryEffect();
        player.MoveSpeedBonus += secondaryCharSpeed;
        player.DashSpeedModifier += secondaryDashSpeed;
        player.DashDurationModifier += secondaryDashDuration;
        player.DashCooldownModifier += secondaryDashCooldown;
    }

    public override void RemoveSecondaryEffect()
    {
        base.RemoveSecondaryEffect();
        player.MoveSpeedBonus -= secondaryCharSpeed;
        player.DashSpeedModifier -= secondaryDashSpeed;
        player.DashDurationModifier -= secondaryDashDuration;
        player.DashCooldownModifier -= secondaryDashCooldown;
    }
}
